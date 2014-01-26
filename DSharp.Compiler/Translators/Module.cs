using System;
using System.Collections.Generic;
using System.Text;
using D_Parser.Dom;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;

namespace DSharp.Compiler.Translators
{
	public class DModuleCompiler
	{
		public DModuleCompiler()
		{
			TypeResolver = new DTypeResolver { ModCmp=this };
			MethodBuilder = new DMethodBuilder { ModCmp=this };
		}

		public AssemblyBuilder AssemblyBuilder;
		public ISymbolDocumentWriter DebugInfo;

		public DTypeResolver TypeResolver;
		public DMethodBuilder MethodBuilder;

		public void Generate(ModuleBuilder mb,DModule Module)
		{
			/*
			 * How to do this:
			 * First generate types in our assembly - so we'll be able to use them while analysing method contents
			 */

			// Resolve imports
			

			//TODO: Build types
			
			MethodInfo entryPoint = null;

			foreach (var e in Module)
			{
				if (e is DMethod)
				{
					var dm = e as DMethod;

					var method=MethodBuilder.GenGlobalMethod(mb, dm);

					//TODO: Also types' member functions shall be entrypoints!
					if (dm.IsEntryFunction)
						entryPoint=method;
				}
			}

			mb.CreateGlobalFunctions();

			if (entryPoint != null)
				AssemblyBuilder.SetEntryPoint(entryPoint);
		}
	}
}
