using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharp.Compiler;
using System.IO;
using System.Reflection;
using D_Parser.Parser;

namespace DSharp.Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			var testContent=res.test0;

			var mod= DParser.ParseString(testContent);

			mod.FileName = "testModule.d";
			mod.Name = "testModule";

			var cmp = new Compiler.Compiler();

			var assemblyName="TestModule";

			var targetFile=Path.ChangeExtension(assemblyName,".exe");

			var ass=cmp.Build(
				assemblyName, 
				targetFile, 
				new[]{
					mod
				}, null
				/*new[]{ 
					,
					Assembly.ReflectionOnlyLoad("System.Data.dll"),
				}*/);
			
			ass.Save(targetFile);
		}
	}
}
