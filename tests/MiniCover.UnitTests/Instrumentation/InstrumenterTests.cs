using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using Mono.Cecil.Tests;
using Shouldly;
using Xunit;

namespace MiniCover.UnitTests
{
    public class InstrumenterTests:BaseTestFixture
    {
		[Fact]
	    public void EncapsulateWithTryFinallyShould_Not_EncapsulateCtor_with_a_field_initialized_out_side_the_constructor()
		{
			var expectedIl = @".locals ()
								IL_0000: ldarg.0 // this
								IL_0001: ldc.i4.5
								IL_0002: stfld System.Int32 AClassWithFieldInitializedOutsideConstructor::value
								IL_0007: ldarg.0 // this
								IL_0008: call System.Void System.Object::.ctor()
								IL_000d: ret";

			TestCSharp("Constructors.cs", module =>
			{
				var type = module.GetType("AClassWithFieldInitializedOutsideConstructor");
				var constructor = type.GetMethod(".ctor");
				Assert.NotNull(constructor);

				Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
			});
		}

        [Fact]
        public void EncapsulateWithTryFinallyShould_Not_EncapsulateCtor_with_existing_try_finally_from_dll()
        {
            var expectedIl = @".locals init ()

// [6 6 - 9 52]
IL_0000: ldarg.0 // this
IL_0001: call System.Void System.Object::.ctor()
.try
{
IL_0006: nop
IL_0007: nop

// [7 7 - 9 10]
IL_0008: nop
.try
{

// [9 9 - 13 14]
IL_0009: nop

// [10 10 - 17 27]
IL_000a: ldarg.0 // this
IL_000b: ldc.i4.5
IL_000c: stfld System.Int32 Sample.TryFinally.AClassWithATryFinallyInConstructor::value

// [11 11 - 13 14]
IL_0011: nop
IL_0012: leave.s IL_001e
}
finally
{

// [13 13 - 13 14]
IL_0014: nop

// [14 14 - 17 24]
IL_0015: ldarg.0 // this
IL_0016: call System.Void Sample.TryFinally.AClassWithATryFinallyInConstructor::Exit()
IL_001b: nop

// [15 15 - 13 14]
IL_001c: nop

// [16 16 - 9 10]
IL_001d: endfinally
IL_001e: leave.s IL_0022
}
finally
{
IL_0020: nop
IL_0021: endfinally
}
IL_0022: ret";

            TestModule("Sample.dll", module =>
            {
                var type = module.GetType("Sample.TryFinally.AClassWithATryFinallyInConstructor");
                var constructor = type.GetMethod(".ctor");
                Assert.NotNull(constructor);
                ApplyTryFinally(constructor, module);
                Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
            }, readOnly:true ,symbolReaderProvider: typeof (PdbReaderProvider), symbolWriterProvider: typeof (PdbWriterProvider));
        }

        [Fact]
        public void ValidFormatShould_Not_EncapsulateCtor_with_existing_try_finally()
        {
            var expectedIl = @".locals init ()
IL_0000: ldarg.0 // this
IL_0001: call System.Void System.Object::.ctor()
.try
{
IL_0006: ldarg.0 // this
IL_0007: ldc.i4.5
IL_0008: stfld System.Int32 AClassWithATryFinallyInConstructor::value
IL_000d: leave.s IL_0016
}
finally
{
IL_000f: ldarg.0 // this
IL_0010: call System.Void AClassWithATryFinallyInConstructor::Exit()
IL_0015: endfinally
}
IL_0016: ret";

            TestCSharp("Constructors.cs", module =>
            {
                var type = module.GetType("AClassWithATryFinallyInConstructor");
                var constructor = type.GetMethod(".ctor");
                Assert.NotNull(constructor);

                Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
            });
        }

	    [Fact]
	    public void EncapsulateWithTryFinallyShould_Not_EncapsulateCtor_with_existing_try_finally()
	    {
		    var expectedIl = @".locals init ()
IL_0000: ldarg.0 // this
IL_0001: call System.Void System.Object::.ctor()
.try
{
IL_0006: nop
.try
{
IL_0007: ldarg.0 // this
IL_0008: ldc.i4.5
IL_0009: stfld System.Int32 AClassWithATryFinallyInConstructor::value
IL_000e: leave.s IL_0017
}
finally
{
IL_0010: ldarg.0 // this
IL_0011: call System.Void AClassWithATryFinallyInConstructor::Exit()
IL_0016: endfinally
IL_0017: leave.s IL_001b
}
finally
{
IL_0019: nop
IL_001a: endfinally
}
IL_001b: ret";

		    TestCSharp("Constructors.cs", module =>
		    {
			    var type = module.GetType("AClassWithATryFinallyInConstructor");
			    var constructor = type.GetMethod(".ctor");
			    Assert.NotNull(constructor);
                ApplyTryFinally(constructor, module);
			    Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
		    }, readOnly:true);
	    }


        private void ApplyTryFinally(MethodDefinition method, ModuleDefinition moduleDefinition)
        {
            var processor = method.Body.GetILProcessor();
            var firstInstruction = GetFirstNonConstructorInstruction(method);
            EncapsulateMethodBodyWithTryFinallyBlock(processor, moduleDefinition, method, firstInstruction);
            processor.Body.OptimizeMacros();
        }

        private Instruction GetFirstNonConstructorInstruction(MethodDefinition method)
        {
            var constructorInstruction = method.Body.Instructions.First(a => a.OpCode == OpCodes.Call);
            return constructorInstruction;

        }
        private void EncapsulateMethodBodyWithTryFinallyBlock(ILProcessor ilProcessor,
            ModuleDefinition assemblyDefinition, MethodDefinition methodDefinition, Instruction firstInstruction)
        {
            var returnEnd =  FixReturns(methodDefinition, assemblyDefinition);

            //var (returnStart, returnEnd) = FixReturns(assemblyDefinition, methodDefinition, ilProcessor);


            var beforeReturn = Instruction.Create(OpCodes.Endfinally);
            ilProcessor.InsertBefore(returnEnd, beforeReturn);

            /////////////// Start of try block  
            Instruction nopInstruction1 = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertAfter(firstInstruction, nopInstruction1);
            //////// Start Finally block
            Instruction nopInstruction2 = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(beforeReturn, nopInstruction2);

            //ilProcessor.InsertBefore(beforeReturn, exitMethodInstruction);
            //ilProcessor.InsertBefore(exitMethodInstruction, loadMethodContextInstruction);

            var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = nopInstruction1,
                TryEnd = nopInstruction2,
                HandlerStart = nopInstruction2,
                HandlerEnd = returnEnd,
            };

            methodDefinition.Body.ExceptionHandlers.Add(handler);
        }

        private static Instruction FixReturns(MethodDefinition methodDefinition, ModuleDefinition assemblyDefinition)
        {
            MethodBody body = methodDefinition.Body;

            Instruction formallyLastInstruction = body.Instructions.Last();
            Instruction lastLeaveInstruction = null;
            if (methodDefinition.ReturnType == assemblyDefinition.TypeSystem.Void)
            {
                var instructions = body.Instructions;
                var lastRet = Instruction.Create(OpCodes.Ret);
                instructions.Add(lastRet);

                for (var index = 0; index < instructions.Count - 1; index++)
                {
                    var instruction = instructions[index];
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        Instruction leaveInstruction = Instruction.Create(OpCodes.Leave, lastRet);
                        if (instruction == formallyLastInstruction)
                        {
                            lastLeaveInstruction = leaveInstruction;
                        }

                        instructions[index] = leaveInstruction;
                    }
                }

                FixBranchTargets(lastLeaveInstruction, formallyLastInstruction, body);
                return lastRet;
            }
            else
            {
                var instructions = body.Instructions;
                var returnVariable = new VariableDefinition(methodDefinition.ReturnType);
                body.Variables.Add(returnVariable);
                var lastLd = Instruction.Create(OpCodes.Ldloc, returnVariable);
                instructions.Add(lastLd);
                instructions.Add(Instruction.Create(OpCodes.Ret));

                for (var index = 0; index < instructions.Count - 2; index++)
                {
                    var instruction = instructions[index];
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        Instruction leaveInstruction = Instruction.Create(OpCodes.Leave, lastLd);
                        if (instruction == formallyLastInstruction)
                        {
                            lastLeaveInstruction = leaveInstruction;
                        }

                        instructions[index] = leaveInstruction;
                        instructions.Insert(index, Instruction.Create(OpCodes.Stloc, returnVariable));
                        index++;
                    }
                }

                FixBranchTargets(lastLeaveInstruction, formallyLastInstruction, body);
                return lastLd;
            }
        }

        private static void FixBranchTargets(
          Instruction lastLeaveInstruction,
          Instruction formallyLastRetInstruction,
          MethodBody body)
        {
            for (var index = 0; index < body.Instructions.Count - 2; index++)
            {
                var instruction = body.Instructions[index];
                if (instruction.Operand != null && instruction.Operand == formallyLastRetInstruction)
                {
                    instruction.Operand = lastLeaveInstruction;
                }
            }
        }


    }

    
}