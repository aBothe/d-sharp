using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using D_Parser.Dom;
using D_Parser.Dom.Statements;

namespace DSharp.Compiler.Translators
{
	public class DStatementCompiler
	{
		public readonly DExpressionCompiler ExpressionCompiler;
		public DMethod CompiledMethod;
		public DMethodLocalsGenerator Locals;
		public readonly ILGenerator il;
		public readonly DModuleCompiler ModCmp;

		public DStatementCompiler(DModuleCompiler ModCmp,ILGenerator Gen)
		{
			this.ModCmp = ModCmp;
			il = Gen;
			ExpressionCompiler = new DExpressionCompiler { StatementCompiler=this };
		}

		public void GenMethodBody(DMethod Method)
		{
			CompiledMethod = Method;

			Locals = new DMethodLocalsGenerator(ModCmp.TypeResolver,this);

			Locals.GenMethodLocals();

			Statement(Method.Body);

			il.Emit(OpCodes.Ret);
		}

		public void Statement(IStatement stmt)
		{
			if (stmt is ExpressionStatement)
			{
				var es = stmt as ExpressionStatement;

				bool neg = false;
				ExpressionCompiler.Expression(stmt, es.Expression, out neg, false);

				// Negation insignificant - throw error
			}

			else if (stmt is BlockStatement)
				foreach (var s in (stmt as BlockStatement))
					Statement(s);

			// Execute initializers
			else if (stmt is DeclarationStatement)
			{
				foreach (var decl in (stmt as DeclarationStatement).Declarations)
				{
					if (!(decl is DVariable))
						continue;

					var dv = decl as DVariable;

					if (dv.Initializer != null)
					{
						var local=Locals.TryGetLocal(stmt, dv.Name);

						// Get the final expression onto the stack
						ExpressionCompiler.Expression(stmt,dv.Initializer);

						// Cast the intializer to our local's type
						ExpressionCompiler.HandleImplicitConversion(stmt,ExpressionCompiler.ResolveExpressionType(stmt,dv.Initializer),local.LocalType);

						// Save it to the local
						il.Emit(OpCodes.Stloc, local);
					}
					//TODO: Error handling if 'auto' type but empty initializer, or on wrong initializer value format etc.
				}
			}


			else if (stmt is LabeledStatement)
			{

			}


			else if (stmt is IfStatement)
			{
				var ifStmt = stmt as IfStatement;

				bool neg = false;
				ExpressionCompiler.Expression(stmt,ifStmt.IfCondition,out neg, true);

				var elseLabel = il.DefineLabel();

				if (ifStmt.IfVariable != null)
				{
					var ifVar= Locals.TryGetLocal(stmt, ifStmt.IfVariable.Name);

					// Duplicate the result so we can use it twice (saving into local + checking its value)
					il.Emit(OpCodes.Dup); 

					// Store content into local
					il.Emit(OpCodes.Stloc, ifVar);
				}

				// If false or null (true or non-null on result negation), jump to else {} code
				il.Emit(neg? OpCodes.Brtrue_S: OpCodes.Brfalse_S, elseLabel);

				// Compile ThenStatement
				Statement(ifStmt.ThenStatement);

				// If else statement given..
				if (ifStmt.ElseStatement != null)
				{
					var followingCode = il.DefineLabel();

					// If ThenStatement executed before, skip ElseStatement
					il.Emit(OpCodes.Br_S, followingCode);

					// If condition will be false, this label will be targeted then
					il.MarkLabel(elseLabel);

					// Compile ElseStatement
					Statement(ifStmt.ElseStatement);

					il.MarkLabel(followingCode);
				}
				else // Let elseLabel go on with following code
					il.MarkLabel(elseLabel);
			}


		}
	}
}
