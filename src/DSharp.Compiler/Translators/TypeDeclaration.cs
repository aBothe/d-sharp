using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D_Parser.Dom;
using System.Reflection;
using D_Parser.Parser;

namespace DSharp.Compiler.Translators
{
	public class DTypeResolver
	{
		public DModuleCompiler ModCmp;

		public Type ResolveType(ITypeDeclaration td)
		{
			if (td is DTokenDeclaration)
			{
				var tok = (td as DTokenDeclaration).Token;

				switch (tok)
				{
					case DTokens.Bool:
						return typeof(bool);
					case DTokens.Ubyte:
						return typeof(byte);
					case DTokens.Byte:
						return typeof(sbyte);
					case DTokens.Ushort:
						return typeof(ushort);
					case DTokens.Short:
						return typeof(short);
					case DTokens.Uint:
						return typeof(uint);
					case DTokens.Int:
						return typeof(int);
					case DTokens.Ulong:
						return typeof(ulong);
					case DTokens.Long:
						return typeof(long);

					case DTokens.Float:
						return typeof(float);
					case DTokens.Double:
						return typeof(double);
					case DTokens.Real:
						return typeof(decimal);

					case DTokens.Char:
						return typeof(byte);
					case DTokens.Wchar:
						return typeof(char);
					case DTokens.Dchar:
						return typeof(uint);


					case DTokens.True:
					case DTokens.False:
						return typeof(bool);

					case DTokens.Void:
						return typeof(void);
				}

				//TODO:
				return Type.ReflectionOnlyGetType(DTokens.GetTokenString(tok),false,false);
			}

			if (td is IdentifierDeclaration)
			{
				Type ret = null;
				ITypeDeclaration remTd=null;
				var id = GetDottedString(td, out remTd);

				if (remTd == null)
				{
					// First test entire string
					ret=Type.GetType(id);

					if (ret != null)
						return ret;

					// Then use imported assemblies
					
				}
			}

			if (td is ArrayDecl)
			{
				var ad = td as ArrayDecl;

				var valueType=ResolveType(ad.ValueType);

				if (valueType!=null && ad.KeyType is DTokenDeclaration && (ad.KeyType as DTokenDeclaration).Token == DTokens.Int)
					return valueType.MakeArrayType();
			}

			return null;
		}

		public static string GetDottedString(ITypeDeclaration decl, out ITypeDeclaration RemainingPrefixes)
		{
			RemainingPrefixes = null;
			var ret = "";

			var curDecl = decl;

			while (curDecl != null)
			{
				if (decl is IdentifierDeclaration && !(decl is DTokenDeclaration))
				{
					ret = (decl as IdentifierDeclaration).Id +'.'+ret;
				}
				else
				{
					RemainingPrefixes = decl;
					break;
				}

				curDecl = curDecl.InnerDeclaration;
			}

			return ret.TrimEnd('.');
		}
	}
}
