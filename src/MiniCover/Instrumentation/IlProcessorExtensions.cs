using System;
using System.Linq;

namespace Mono.Cecil.Cil
{
    public static class IlProcessorExtensions
    {
        internal static void EncapsulateMethodBodyWithTryFinallyBlock(this ILProcessor ilProcessor,
            Instruction firstInstruction, Action<ILProcessor, Instruction> insertBeforReturn)
        {
            var body = ilProcessor.Body;
            if (body.Method.IsConstructor)
            {
                var ctor = GetFirstConstructorInstruction(body);
                if (body.Instructions.IndexOf(ctor) > 2)
                {
                    var lastInstruction = body.Instructions.Last();
                    EncapsulateWithTryCatch(ilProcessor, firstInstruction, ctor.Previous);
                    if (ctor.Next.Equals(lastInstruction)) return;
                }

                if(firstInstruction.Next.OpCode != OpCodes.Nop)
                {
                    firstInstruction = Instruction.Create(OpCodes.Nop);
                    ilProcessor.InsertAfter(ctor, firstInstruction);
                }
            }
            
            var returnStart = ilProcessor.FixReturns();

            var beforeReturn = Instruction.Create(OpCodes.Endfinally);
            ilProcessor.InsertBefore(returnStart, beforeReturn);

            Instruction finallyStart = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(beforeReturn, finallyStart);
            insertBeforReturn(ilProcessor, beforeReturn);

            var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = firstInstruction,
                TryEnd = finallyStart,
                HandlerStart = finallyStart,
                HandlerEnd = returnStart,
            };

            body.ExceptionHandlers.Add(handler);
        }

        internal static void EncapsulateWithTryCatch(ILProcessor ilProcessor, Instruction from,
            Instruction to)
        {
            var methodDefinition = ilProcessor.Body.Method;
            var tryEnd = Instruction.Create(OpCodes.Leave_S, to);
            ilProcessor.InsertBefore(to, tryEnd);

            Instruction tryStart = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(from, tryStart);
			
            Instruction rethrowInstruction = Instruction.Create(OpCodes.Rethrow);
            ilProcessor.InsertAfter(tryEnd, rethrowInstruction);
            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = tryStart,
                TryEnd = rethrowInstruction,
                HandlerStart = rethrowInstruction,
                HandlerEnd = to,
                CatchType = methodDefinition.Module.ImportReference(typeof(Exception))
            };
            methodDefinition.Body.ExceptionHandlers.Add(handler);
        }

        private static Instruction GetFirstConstructorInstruction(MethodBody body)
        {
            var constructorInstruction = body.Instructions.First(a => a.OpCode == OpCodes.Call);
            return constructorInstruction;
        }

        internal static Instruction FixReturns(this ILProcessor ilProcessor)
        {
            var methodDefinition = ilProcessor.Body.Method;
            if (methodDefinition.ReturnType == methodDefinition.Module.TypeSystem.Void)
            {
                var instructions = ilProcessor.Body.Instructions.ToArray();

                var newReturnInstruction = ilProcessor.Create(OpCodes.Ret);
                ilProcessor.Append(newReturnInstruction);

                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        var leaveInstruction = ilProcessor.Create(OpCodes.Leave, newReturnInstruction);
                        ilProcessor.Replace(instruction, leaveInstruction);

                        ilProcessor.ReplaceInstructionReferences(instruction, leaveInstruction);
                    }
                }

                return newReturnInstruction;
            }
            else
            {
                var instructions = ilProcessor.Body.Instructions.ToArray();

                var returnVariable = new VariableDefinition(methodDefinition.ReturnType);
                ilProcessor.Body.Variables.Add(returnVariable);

                var loadResultInstruction = ilProcessor.Create(OpCodes.Ldloc, returnVariable);
                ilProcessor.Append(loadResultInstruction);
                var newReturnInstruction = ilProcessor.Create(OpCodes.Ret);
                ilProcessor.Append(newReturnInstruction);

                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        var saveResultInstruction = ilProcessor.Create(OpCodes.Stloc, returnVariable);
                        ilProcessor.Replace(instruction, saveResultInstruction);
                        var leaveInstruction = ilProcessor.Create(OpCodes.Leave, loadResultInstruction);
                        ilProcessor.InsertAfter(saveResultInstruction, leaveInstruction);

                        ilProcessor.ReplaceInstructionReferences(instruction, leaveInstruction);
                    }
                }

                return loadResultInstruction;
            }
        }

        internal static void RemoveTailInstructions(this ILProcessor ilProcessor)
        {
            foreach (var instruction in ilProcessor.Body.Instructions.ToArray())
            {
                if (instruction.OpCode == OpCodes.Tail)
                {
                    var noOpInstruction = ilProcessor.Create(OpCodes.Nop);
                    ilProcessor.Replace(instruction, noOpInstruction);
                    ReplaceInstructionReferences(ilProcessor, instruction, noOpInstruction);
                }
            }
        }

        internal static void ReplaceInstructionReferences(this ILProcessor ilProcessor,
            Instruction oldInstruction,
            Instruction newInstruction)
        {
            //change try/finally etc to point to our first instruction if they referenced the one we inserted before
            foreach (var handler in ilProcessor.Body.ExceptionHandlers)
            {
                if (handler.FilterStart == oldInstruction)
                    handler.FilterStart = newInstruction;

                if (handler.TryStart == oldInstruction)
                    handler.TryStart = newInstruction;
                if (handler.TryEnd == oldInstruction)
                    handler.TryEnd = newInstruction;

                if (handler.HandlerStart == oldInstruction)
                    handler.HandlerStart = newInstruction;
                if (handler.HandlerEnd == oldInstruction)
                    handler.HandlerEnd = newInstruction;
            }

            //change instructions with a target instruction if they referenced the one we inserted before to be our first instruction
            foreach (var iteratedInstruction in ilProcessor.Body.Instructions)
            {
                var operand = iteratedInstruction.Operand;
                if (operand == oldInstruction)
                {
                    iteratedInstruction.Operand = newInstruction;
                    continue;
                }

                if (!(operand is Instruction[]))
                    continue;

                var operands = (Instruction[])operand;
                for (var i = 0; i < operands.Length; ++i)
                {
                    if (operands[i] == oldInstruction)
                        operands[i] = newInstruction;
                }
            }
        }
    }
}