using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using D_Parser.Dom.Expressions;
using DSharp.Parser;
using System.Reflection;
using D_Parser.Dom.Statements;

namespace DSharp.Compiler.Translators
{
	public class DExpressionCompiler
	{
		public ILGenerator il
		{
			get { return StatementCompiler.il; }
		}
		public DStatementCompiler StatementCompiler;
		public DModuleCompiler ModCmp { get { return StatementCompiler.ModCmp; } }

		public void Expression(IStatement Scope,
				IExpression ex,
				bool KeepResultOnStack = true)
		{
			bool neg = false;

			Expression(Scope, ex, out neg, KeepResultOnStack);

			if (neg)
				EmitNegation();
		}

		void EmitNegation()
		{
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ceq);
		}

		/// <summary>
		/// Evaluates an expression and pushs its return value ontop of the stack.
		/// </summary>
		/// <param name="Scope"></param>
		/// <param name="ex"></param>
		public void Expression(IStatement Scope,
				IExpression ex,out bool RequiresOuterNegation,
				bool KeepResultOnStack=true)
		{
			RequiresOuterNegation = false;

			if (ex is Expression)
			{
				var expressions = (ex as Expression).Expressions;

				var nextCode = il.DefineLabel();

				for (int i = 0; i < expressions.Count; i++)
				{
					bool isLastIteration = i == expressions.Count - 1;

					bool neg = false;
					Expression(Scope, expressions[i], out neg, isLastIteration?KeepResultOnStack:true);

					// If it's not the last expression..
					if (!isLastIteration)
						// To see if it's false, and if so, skip the execution of following expression
						il.Emit(neg? OpCodes.Brtrue_S: OpCodes.Brfalse_S, nextCode);
				}

				il.MarkLabel(nextCode);
				return;
			}


			#region AssignExpression
			if (ex is AssignExpression)
			{
				var aex = ex as AssignExpression;

				// Evaluate rValue
				Expression(Scope, aex.RightOperand);

				// Try to implicitly cast the right operand to the type of the left operand
				HandleImplicitConversion(Scope, aex.RightOperand, aex.LeftOperand);

				switch (aex.OperatorToken)
				{
						// a = b;
					case DTokens.Assign:
						// No extra action required
						break;

					case DTokens.PlusAssign: // +=
						// Push lValue's value
						Expression(Scope, aex.LeftOperand);

						// Add both values
						il.Emit(OpCodes.Add);
						break;
					case DTokens.MinusAssign:
						Expression(Scope, aex.LeftOperand);
						il.Emit(OpCodes.Sub);
						break;
					case DTokens.TimesAssign:
						Expression(Scope, aex.LeftOperand);
						il.Emit(OpCodes.Mul);
						break;
					case DTokens.DivAssign:
						Expression(Scope, aex.LeftOperand);
						il.Emit(OpCodes.Div);
						break;
					case DTokens.ModAssign:
						Expression(Scope, aex.LeftOperand);
						il.Emit(OpCodes.Rem);
						break;

					case DTokens.BitwiseAndAssign:
						break;
					case DTokens.BitwiseOrAssign:
						break;
					case DTokens.XorAssign:
						break;

					case DTokens.ShiftLeftAssign:
						break;
					case DTokens.ShiftRightAssign:
						break;
					case DTokens.TripleRightShiftAssign:
						break;


					case DTokens.TildeAssign: // a ~= b; // Array concatination!
						break;
				}

				if(KeepResultOnStack)
					il.Emit(OpCodes.Dup);

				StoreStackAt(Scope, aex.LeftOperand);
				return;
			}
			#endregion





			if (ex is ConditionalExpression)
			{
				var cex = ex as ConditionalExpression;

				return;
			}

			if (ex is OrOrExpression)
			{
				return;
			}


			if (ex is AndAndExpression)
			{ return; }


			if (ex is XorExpression)
			{ return; }

			if (ex is OrExpression)
			{ return; }

			if (ex is AndExpression)
			{ return; }

			if (ex is EqualExpression)
			{
				var eq = ex as EqualExpression;

				Expression(Scope, eq.RightOperand);

				Expression(Scope, eq.LeftOperand);

				il.Emit(OpCodes.Ceq);

				if (eq.OperatorToken == DTokens.NotEqual)
				{
					// Compare against 0
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
				}

				return;
			}


			#region Unary expressions

			if (ex is UnaryExpression_Not)
			{
				bool not = false;
				Expression(Scope, (ex as UnaryExpression_Not).UnaryExpression,out not);

				if (!not) // Only request outer negation if not double-negated (so like !!a but on !!!a or !a)
					RequiresOuterNegation=true;
				return;
			}

			// +5 
			if (ex is UnaryExpression_Add)
			{
				Expression(Scope, (ex as UnaryExpression_Add).UnaryExpression);
				return;
			}

			// -5
			if (ex is UnaryExpression_Sub)
			{
				var uex=ex as UnaryExpression_Sub;

				Expression(Scope, uex.UnaryExpression);

				il.Emit(OpCodes.Neg);
				return;
			}

			#endregion

			if (ex is NewExpression)
			{
				var nex = ex as NewExpression;

				// Search for instanciated type
				var instType= ModCmp.TypeResolver.ResolveType(nex.Type);

				if (instType == null)
				{
					// Error: No type found
					return;
				}

				// Eval argument types
				var paramTypes = new List<Type>();

				foreach (var arg in nex.Arguments)
				{
					var parT = ResolveExpressionType(Scope, arg);

					if (parT == null)
					{
						// Error: Empty/Illegal expression type
						return;
					}

					paramTypes.Add(parT);
				}

				// Get ctor method
				var ctor=instType.GetConstructor(paramTypes.ToArray());

				if (ctor == null)
				{
					// Error: Constructor not found
					return;
				}

				il.Emit(OpCodes.Newobj, ctor);
			}


			#region Postfix expressions

			if (ex is PostfixExpression_MethodCall)
			{
				var mc = ex as PostfixExpression_MethodCall;

				// Push all parameters
				var paramTypes = new List<Type>();
				if (mc.ArgumentCount > 0)
					foreach (var arg in mc.Arguments)
					{
						var paramType = ResolveExpressionType(Scope, arg);

						// TODO: paramType NullCheck

						// Push arg onto stack
						Expression(Scope, arg);

						paramTypes.Add(paramType);
					}

				// Drop method call
				// Currently static functions only --- like Console.WriteLine

				// Build method name
				var typeName = "";
				var methodName = "";

				var curEx = mc.PostfixForeExpression;

				if (curEx is PostfixExpression_Access)
				{
					methodName = (curEx as PostfixExpression_Access).TemplateOrIdentifier.ToString(false);
					curEx = (curEx as PostfixExpression).PostfixForeExpression;

					while (curEx != null)
					{
						if (curEx is PostfixExpression_Access)
						{
							typeName = '.' + (curEx as PostfixExpression_Access).TemplateOrIdentifier.ToString(false) + typeName;

							curEx = (curEx as PostfixExpression_Access).PostfixForeExpression;
						}
						else if (curEx is IdentifierExpression)
						{
							var id = curEx as IdentifierExpression;
							if (id.IsIdentifier)
								typeName = (id.Value as string) + typeName;

							break;
						}
						else
						{
							typeName = null;
							break;
						}
					}
				}
				else return;

				if (string.IsNullOrEmpty(typeName))
					return;

				var type = Type.GetType(typeName, false);

				if (type == null)
					return;

				var met = type.GetMethod(methodName, paramTypes.ToArray());

				il.EmitCall(OpCodes.Call, met, null);

				// If returned value is not void and is not needed furthermore, pop it
				if (met.ReturnType != typeof(void))
				{
					if (!KeepResultOnStack)
						il.Emit(OpCodes.Pop);
				}
				else
				{
					//HACK: If void type but result expected, push a 'true' value onto the stack 
					if (KeepResultOnStack)
						il.Emit(OpCodes.Ldc_I4_1);
				}

				return;
			}

			#endregion


			#region Primary expressions

			if (ex is IsExpression)
			{

			}

			if (ex is SurroundingParenthesesExpression)
			{
				Expression(Scope,(ex as SurroundingParenthesesExpression).Expression);
				return;
			}

			if (ex is IdentifierExpression)
			{
				if (!KeepResultOnStack)
				{
					//Error: 'Empty' constant expression is senseless (e.g.  "ABC String"; )
					return;
				}

				var ie = ex as IdentifierExpression;

				if (ie.IsIdentifier)
				{
					var local = StatementCompiler.Locals.TryGetLocal(Scope, ie.Value as string);

					if (local != null)
					{
						il.Emit(OpCodes.Ldloc_S, local);
						return;
					}

					//TODO: Search in arguments, object properties, base type properties, global properties for adequate Type
					return;
				}

				// If it's a const string, push it onto the stack
				if (ie.Format == LiteralFormat.StringLiteral || ie.Format == LiteralFormat.VerbatimStringLiteral)
				{
					il.Emit(OpCodes.Ldstr, ie.Value as string);
					return;
				}

				if (ie.Format == LiteralFormat.CharLiteral)
				{
					if (ie.Value is char)
						il.Emit(OpCodes.Ldind_U2, (char)ie.Value);
					//TODO: What is there to do if there's a surrogate pair of chars? (mainly on UTF32/dchar usage) -- Remove utf32 support?
					return;
				}

				if (ie.Format == LiteralFormat.FloatingPoint)
				{
					if (ie.Value is double)
						il.Emit(OpCodes.Ldc_R8, (double)ie.Value);
					else
						il.Emit(OpCodes.Ldc_R4, (float)ie.Value);

					return;
				}

				if (ie.Format == LiteralFormat.Scalar)
				{
					//TODO: Complete for all scalar types
					//TODO: decimals
					if (ie.Value is int)
						il.Emit(OpCodes.Ldc_I4, (int)ie.Value);
					else if (ie.Value is uint)
						il.Emit(OpCodes.Ldc_I4, (int)ie.Value);
					else if (ie.Value is long)
						il.Emit(OpCodes.Ldc_I8, (long)ie.Value);
					else if (ie.Value is ulong)
						il.Emit(OpCodes.Ldc_I8, (long)ie.Value);

					//TODO: How to handle complex numbers?
					return;
				}

				return;
			}

			if (ex is TokenExpression)
			{
				var tok = (ex as TokenExpression).Token;

				switch (tok)
				{
					case DTokens.True:
						il.Emit(OpCodes.Ldc_I4_1);
						break;
					case DTokens.False:
						il.Emit(OpCodes.Ldc_I4_0);
						break;

					case DTokens.Null:
						il.Emit(OpCodes.Ldnull);
						break;
				}

				return;
			}

			#endregion

			// Expressions not handled yet:
			throw new NotImplementedException(ex.ToString() +" not compilable yet!");
		}

		public void StoreStackAt(IStatement Scope, IExpression lValue)
		{
			if (lValue is IdentifierExpression)
			{
				var local = StatementCompiler.Locals.TryGetLocal(Scope, (lValue as IdentifierExpression).Value as string);

				il.Emit(OpCodes.Stloc, local);
			}
		}

		public void HandleImplicitConversion(IStatement Scope, IExpression lValue, IExpression rValue)
		{
			HandleImplicitConversion(Scope,ResolveExpressionType(Scope,lValue), ResolveExpressionType(Scope,rValue));
		}

		/// <summary>
		/// Checks if currently stacked value needs to be converted.
		/// </summary>
		/// <param name="Scope"></param>
		/// <param name="originType"></param>
		/// <param name="targetType"></param>
		public void HandleImplicitConversion(IStatement Scope, Type originType, Type targetType)
		{
			// If origin equals target, do nothing
			if (originType == targetType)
				return;

			// ValueType conversions
			if (originType.IsValueType)
			{
				if (targetType == typeof(bool))
					il.Emit(OpCodes.Conv_U1);

				else if (targetType == typeof(byte))
					il.Emit(OpCodes.Conv_U1);
				else if (targetType == typeof(sbyte))
					il.Emit(OpCodes.Conv_I1);

				else if (targetType == typeof(short))
					il.Emit(OpCodes.Conv_I2);
				else if (targetType == typeof(ushort))
					il.Emit(OpCodes.Conv_I2);

				else if (targetType == typeof(int))
					il.Emit(OpCodes.Conv_I4);
				else if (targetType == typeof(uint))
					il.Emit(OpCodes.Conv_U4);

				else if (targetType == typeof(long))
					il.Emit(OpCodes.Conv_I8);
				else if (targetType == typeof(ulong))
					il.Emit(OpCodes.Conv_U8);

				else if (targetType == typeof(float))
					il.Emit(OpCodes.Conv_R4);
				else if (targetType == typeof(double))
					il.Emit(OpCodes.Conv_R8);

				//TODO: Match everything against everything
			}
		}

		public Type ResolveExpressionType(IStatement Scope,IExpression ex)
		{
			if (ex is IdentifierExpression)
			{
				var ie = ex as IdentifierExpression;

				// If it's a scalar value
				if (ie.IsConstant)
					return ie.Value.GetType();

				if (ie.Format == LiteralFormat.StringLiteral)
					return typeof(string);

				if (ie.Format == LiteralFormat.CharLiteral)
					return typeof(char);

				var local = this.StatementCompiler.Locals.TryGetLocal(Scope, ie.Value as string);

				if (local != null)
					return local.LocalType;

				//TODO: Resolve other AST nodes and find out their types
			}

			#region Unary expressions

			if (ex is UnaryExpression_Type)
			{ }

			#endregion

			if (ex is TokenExpression)
			{
				var tok = (ex as TokenExpression).Token;

				switch (tok)
				{
					case DTokens.True:
					case DTokens.False:
						return typeof(bool);

					case DTokens.Null:
						return typeof(Object);
				}
			}

			if (ex is NewExpression)
			{
				var nex = ex as NewExpression;

				return ModCmp.TypeResolver.ResolveType(nex.Type);
			}

			//TODO: Remove incomplete resolution - what if there's a function called and you got to know its return type?
			return ModCmp.TypeResolver.ResolveType(ex.ExpressionTypeRepresentation);
		}
	}
}
