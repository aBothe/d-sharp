.assembly extern mscorlib {}
.assembly Tryout1 {}

.method static public void LOLmain(string[] args) il managed
{
	.entrypoint
	
	.maxstack 8
	
	.locals init(int32 i, bool a, bool b)
	
	ldc.i4.1
	ldc.i4.0
	ceq
	brfalse.s iter
	call void [mscorlib]System.Console::WriteLine(uint32)
	
	// Load first argument
	
	iter: nop
	ldarg.0
	ldc.i4.0
	ldelem.ref
	call void [mscorlib]System.Console::WriteLine(string)
	
	// Store false into a
	ldc.i4.0
	stloc.s a
	
	// Store true into b
	ldc.i4.1
	stloc.s b
	
	// if(a && b)
	
	// a
	ldloc.s a
	brfalse.s elsecode
	
	// && b
	ldloc.s b
	brfalse.s elsecode
	
	// The result is "true"
	ldstr "true!"
	call void [mscorlib]System.Console::WriteLine(string)
	br.s nextcode
	
	elsecode: ldstr "false!"
	call void [mscorlib]System.Console::WriteLine(string)
	
	
	nextcode:
	ldstr ".. some other code"
	call void [mscorlib]System.Console::WriteLine(string)
	ret
}