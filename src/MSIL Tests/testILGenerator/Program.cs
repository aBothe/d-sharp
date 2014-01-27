using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace testILGenerator
{
	class Program
	{
		//delegate void LolDlg(int lol);

		static void foo(ref int i)
		{
			i = 2;
		}

		static void Main(string[] args)
		{
			Console.WriteLine(typeof(object).FullName);

			var o = new List<string>(args).ToArray();

			Console.WriteLine(o);

			int i = 34;

			foo(ref i);

			Console.WriteLine(i);

			Console.Read();

			/*
			ulong u = 1000000000;

			u += 1;

			bool bb = false;

			if(bb=u!=50)
				Console.WriteLine(u);
			*/
			/*
			LolDlg dlg = delegate(int lol)
			{
				Console.WriteLine("Super delegate!");
			};
			 dlg(12345);
			 */
			/*
			var prg = new Program();

			var asdf = prg.LolProp;

			prg.LolProp = "xD";
			
			asdf = "2";
			*/

			// new B().foo();
			/*
			bool a = args.Length == 0;
			bool b = args.Length > 0;

			bool db;

			bool c = db=(a && b) || (a && !b);
			*/
			/*
			if (c)
			{
				Console.WriteLine("c is true");
				string d = "";

				Console.Write(d);
			}
			else
			{
				Console.WriteLine("c is false");
				string d = "ohyeah";

				Console.Write(d);
			}
			*/
		}
	}
}
