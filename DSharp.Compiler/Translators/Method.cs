using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D_Parser.Dom;
using System.Reflection.Emit;
using System.Reflection;
using DSharp.Parser;
using D_Parser.Dom.Statements;

namespace DSharp.Compiler.Translators
{
	public class DMethodBuilder
	{
		public DModuleCompiler ModCmp;

		public static MethodAttributes GenMethodAttributes(DMethod Method)
		{
			//Note: This is a quick'n dirty implementation only - many things are still left to do
			MethodAttributes a = MethodAttributes.Final;

			if (Method.Body == null)
				a |= MethodAttributes.Abstract;

			foreach (var attr in Method.Attributes)
			{
				switch (attr.Token)
				{
					case DTokens.Public:
						a |= MethodAttributes.Public;
						break;
					case DTokens.Private:
						a |= MethodAttributes.Private;
						break;
					case DTokens.Protected:
						a |= MethodAttributes.Family;
						break;
					case DTokens.Package:
						a |= MethodAttributes.Assembly;
						break;
					case DTokens.Static:
						a |= MethodAttributes.Static;
						break;
				}
			}

			return a;
		}

		public MethodBuilder GenGlobalMethod(ModuleBuilder mb, DMethod Method)
		{
			var parameters = new List<Type>(Method.Parameters.Count);
			
			foreach (var p in Method.Parameters)
			{
				var p_=ModCmp.TypeResolver.ResolveType(p.Type);
				if (p_ != null)
					parameters.Add(p_);
			}

			var attrs=GenMethodAttributes(Method);

			if(Method.IsEntryFunction)
			{
				attrs=MethodAttributes.Static;
			}

			var metb = mb.DefineGlobalMethod(Method.Name, attrs, Method.IsEntryFunction ? typeof(void) : ModCmp.TypeResolver.ResolveType(Method.Type), parameters.Count < 1 ? Type.EmptyTypes : parameters.ToArray());
			
			var il=metb.GetILGenerator();

			var stmtCmp = new DStatementCompiler(ModCmp,il);

			stmtCmp.GenMethodBody(Method);

			return metb;
		}

		public void GenMethod(MethodBuilder mb,DMethod Method)
		{

		}
	}
}
