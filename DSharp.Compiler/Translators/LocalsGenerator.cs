using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D_Parser.Dom;
using System.Reflection.Emit;
using D_Parser.Dom.Statements;
using DSharp.Parser;
using D_Parser.Dom.Expressions;

namespace DSharp.Compiler.Translators
{
	/// <summary>
	/// Enumerates method's locals.
	/// Also scans down expression trees for possibly needed helper variables.
	/// 
	/// Because of declaring method locals at the beginning of a method, the performance can be increased
	/// </summary>
	public class DMethodLocalsGenerator
	{
		DStatementCompiler Cmp;
		DTypeResolver TypeResolver;
		public readonly Dictionary<INode, LocalBuilder> PrimaryLocals = new Dictionary<INode, LocalBuilder>();
		/// <summary>
		/// TODO: Does one expression need more than one helper local?
		/// </summary>
		public readonly Dictionary<IExpression, LocalBuilder> HelperLocals = new Dictionary<IExpression, LocalBuilder>();

		public DMethodLocalsGenerator(DTypeResolver TypeRes,DStatementCompiler MethodCompiler)
		{
			this.TypeResolver = TypeRes;
			Cmp = MethodCompiler;
		}

		/// <summary>
		/// Enums all method body's variables, adds them to a dictionary which will be returned, and declares them by the ILGenerator.
		/// </summary>
		public void GenMethodLocals()
		{
			if (Cmp.CompiledMethod.Body == null)
				return;

			//TODO: In,Out tests
			//TODO: VarArgs

			var l1 = new List<IStatement> { Cmp.CompiledMethod.Body };
			var l2 = new List<IStatement>();

			while (l1.Count > 0)
			{
				foreach (var bs in l1)
				{
					if (bs is IDeclarationContainingStatement)
						foreach (var decl in (bs as IDeclarationContainingStatement).Declarations)
						{
							var dv = decl as DVariable;

							if (dv == null) // TODO: Handle nested methods
								continue;

							//TODO: Aliases

							Type t = null;
							// Handle auto declarations
							if (decl.Type != null)
								t = TypeResolver.ResolveType(decl.Type);
							else if (dv.ContainsAttribute(DTokens.Auto))
								//TODO: Handle empty initializer
								t = Cmp.ExpressionCompiler.ResolveExpressionType(bs,dv.Initializer);

							//TODO: Null-check t - throw error if null

							var localBuilder = Cmp.il.DeclareLocal(t);
							//TODO: Handle variable names that occur more than once

							//TODO: Add symbolic debug info and expression offsets
							//localBuilder.SetLocalSymInfo(decl.Name);

							PrimaryLocals.Add(dv, localBuilder);
						}

					if (bs is StatementContainingStatement)
						l2.AddRange((bs as StatementContainingStatement).SubStatements);

					if (bs is IExpressionContainingStatement)
						foreach (var ex in (bs as IExpressionContainingStatement).SubExpressions)
							GenExpressionHelperLocals(ex);
				}

				l1.Clear();
				l1.AddRange(l1);
				l2.Clear();
			}
		}

		void GenExpressionHelperLocals(IExpression ex)
		{
			//TODO: Create helpers
		}



		public LocalBuilder TryGetLocal(IStatement CurrentStatementLevel, string Name)
		{
			var declLimit=CurrentStatementLevel.StartLocation;

			while (CurrentStatementLevel != null)
			{
				if (CurrentStatementLevel is IDeclarationContainingStatement)
				{
					var dcs = CurrentStatementLevel as IDeclarationContainingStatement;

					if(dcs.Declarations!=null)
						foreach (var decl in dcs.Declarations)
							if (decl.StartLocation <= declLimit && decl.Name == Name)
							{
								LocalBuilder ret = null;

								PrimaryLocals.TryGetValue(decl, out ret);

								return ret;
							}
				}
				CurrentStatementLevel = CurrentStatementLevel.Parent;
			}

			return null;
		}
	}
}
