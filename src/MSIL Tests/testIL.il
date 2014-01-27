
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.4916
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly testILGenerator
{
  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1A 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 30 01 00 54   // ,Version=v4.0..T
                                                                                                        0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C   // ..FrameworkDispl
                                                                                                        61 79 4E 61 6D 65 10 2E 4E 45 54 20 46 72 61 6D   // ayName..NET Fram
                                                                                                        65 77 6F 72 6B 20 34 )                            // ework 4
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0F 74 65 73 74 49 4C 47 65 6E 65 72 61 74   // ...testILGenerat
                                                                                              6F 72 00 00 )                                     // or..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0F 74 65 73 74 49 4C 47 65 6E 65 72 61 74   // ...testILGenerat
                                                                                                6F 72 00 00 )                                     // or..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 12 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  20 32 30 31 31 00 00 )                            //  2011..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 62 32 63 66 35 33 31 38 2D 64 37 62 66   // ..$b2cf5318-d7bf
                                                                                                  2D 34 65 62 62 2D 62 32 39 37 2D 34 35 37 61 38   // -4ebb-b297-457a8
                                                                                                  64 62 36 33 32 65 35 00 00 )                      // db632e5..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module testILGenerator.exe
// MVID: {4EA573D3-A0C1-408B-BAFE-99A8D6D0D6A9}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000003    //  ILONLY 32BITREQUIRED
// Image base: 0x00210000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit testILGenerator.Program
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  foo(int32& i) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.2
    IL_0003:  stind.i4
    IL_0004:  ret
  } // end of method Program::foo

  .method private hidebysig static void  Main(string[] args) cil managed
  {
    .entrypoint
    // Code size       66 (0x42)
    .maxstack  2
    .locals init ([0] string[] o,
             [1] int32 i)
    IL_0000:  nop
    IL_0001:  ldtoken    [mscorlib]System.Object
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  callvirt   instance string [mscorlib]System.Type::get_FullName()
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  nop
    IL_0016:  ldarg.0
    IL_0017:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_001c:  call       instance !0[] class [mscorlib]System.Collections.Generic.List`1<string>::ToArray()
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0028:  nop
    IL_0029:  ldc.i4.s   34
    IL_002b:  stloc.1
    IL_002c:  ldloca.s   i
    IL_002e:  call       void testILGenerator.Program::foo(int32&)
    IL_0033:  nop
    IL_0034:  ldloc.1
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003a:  nop
    IL_003b:  call       int32 [mscorlib]System.Console::Read()
    IL_0040:  pop
    IL_0041:  ret
  } // end of method Program::Main

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Program::.ctor

} // end of class testILGenerator.Program

.class public auto ansi beforefieldinit testILGenerator_OtherNamespace.Class1
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Class1::.ctor

} // end of class testILGenerator_OtherNamespace.Class1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file testIL.res
