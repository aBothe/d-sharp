using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D_Parser.Dom;
using System.Reflection;
using System.Reflection.Emit;
using DSharp.Compiler.Translators;

namespace DSharp.Compiler
{
	public class Compiler
	{
		/// <summary>
		/// In here, 'static' variables like
		/// the current OS version,
		/// the debug build state,
		/// the unittest build state
		/// </summary>
		public Dictionary<string, object> MetaVariables = new Dictionary<string, object>();

		public AssemblyBuilder Build(
			string AssemblyName, 
			string AssemblyTargetFile, 
			IEnumerable<DModule> Modules, 
			IEnumerable<Assembly> AssemblyReferences)
		{
			var targetName = new AssemblyName(AssemblyName);

			var builtAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(targetName, AssemblyBuilderAccess.RunAndSave);

			CompileModules(builtAssembly, AssemblyTargetFile, Modules, AssemblyReferences);
			
			return builtAssembly;
		}

		public ModuleBuilder CompileModules(
			AssemblyBuilder AssemblyBuilder,
			string AssemblyTargetFile, 
			IEnumerable<DModule> Modules, 
			IEnumerable<Assembly> AssemblyReferences)
		{
			/*
			 * We will define only one final module which will contain all types and methods from all given DModules.
			 * 
			 * Note: Subnamespaces will be virtual only, so classes will be named as MySubNamespace.MyClass then if the module's namespace is MySubNamespace
			 */
			var targetModule = AssemblyBuilder.DefineDynamicModule(AssemblyTargetFile, true);

			foreach (var mod in Modules)
			{
				var mb = new DModuleCompiler	{AssemblyBuilder=AssemblyBuilder};

				mb.Generate(targetModule, mod);
			}

			targetModule.CreateGlobalFunctions();
			
			return targetModule;
		}
	}
}
