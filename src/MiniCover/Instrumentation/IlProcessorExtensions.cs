using System;
using System.Linq;

namespace Mono.Cecil.Cil
{
    public static class IlProcessorExtensions
    {
        internal static void ForEachReturn(this ILProcessor ilProcessor, Action<ILProcessor, Instruction> action)
        {
            foreach (var instruction in ilProcessor.Body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToArray())
            {
                action(ilProcessor, instruction);
            }
        }

        internal static void EncapsulateMethodBodyWithTryFinallyBlock(
            this ILProcessor ilProcessor,
            Instruction firstInstruction,
            Action<ILProcessor, Instruction> insertBeforReturn)
        {
            var body = ilProcessor.Body;
            if (body.Method.IsConstructor && !body.Method.IsStatic)
            {
                var ctor = GetFirstConstructorInstruction(body);
                if (ctor != null)
                {
                    if (body.Instructions.IndexOf(ctor) > 2)
                    {
                        var lastInstruction = body.Instructions.Last();
                        var firtLDarg0BeforeCtor = ctor.GetFirstPreviousLdarg_0();
                        if (firstInstruction != firtLDarg0BeforeCtor)
                        {
                            EncapsulateWithTryCatch(ilProcessor, firstInstruction, firtLDarg0BeforeCtor);
                        }

                        if (ctor.GetFirstNotNopInstruction().Equals(lastInstruction))
                        {
                            insertBeforReturn(ilProcessor, lastInstruction);
                            return;
                        }
                    }

                    if (firstInstruction.Next.OpCode != OpCodes.Nop)
                    {
                        firstInstruction = Instruction.Create(OpCodes.Nop);
                        ilProcessor.InsertAfter(ctor, firstInstruction);
                    }
                }
            }

            var returnStart = ilProcessor.FixReturns();

            var beforeReturn = Instruction.Create(OpCodes.Endfinally);
            ilProcessor.InsertBefore(returnStart, beforeReturn);

            if (body.Instructions.First().Equals(firstInstruction))
            {
                Instruction tryStart = Instruction.Create(OpCodes.Nop);
                ilProcessor.InsertBefore(firstInstruction, tryStart);
            }

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

        private static Instruction GetFirstPreviousLdarg_0(this Instruction instruction)
        {
            var previous = instruction.Previous;
            if (previous == null)
            {
                Console.WriteLine(instruction.ToString());
                return instruction;
            }
            if (previous.OpCode == OpCodes.Ldarg_0 || previous.OpCode == OpCodes.Ldarg && previous.Operand is ParameterDefinition && ((ParameterDefinition)previous.Operand).Name == string.Empty) return previous;
            return previous.GetFirstPreviousLdarg_0();
        }

        private static Instruction GetFirstNotNopInstruction(this Instruction instruction)
        {
            var next = instruction.Next;
            if (next.OpCode != OpCodes.Nop) return next;
            return next.GetFirstNotNopInstruction();
        }

        internal static void EncapsulateWithTryCatch(ILProcessor ilProcessor, Instruction from,
            Instruction to)
        {
            var methodDefinition = ilProcessor.Body.Method;
            var tryEnd = Instruction.Create(OpCodes.Leave_S, to);
            ilProcessor.InsertBefore(to, tryEnd);

            Instruction tryStart = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(from, tryStart);
            Instruction beforeTry = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(tryStart, beforeTry);
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
            var constructorInstruction = body.Instructions
                .FirstOrDefault(a => a.OpCode == OpCodes.Call && (a.Operand as MethodReference)?.Name == ".ctor");
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
                        var leaveInstruction = ilProcessor.Create(OpCodes.Leave, loadResultInstruction);
                        ilProcessor.Replace(instruction, leaveInstruction);
                        ilProcessor.ReplaceInstructionReferences(instruction, leaveInstruction);
                        var saveResultInstruction = ilProcessor.Create(OpCodes.Stloc, returnVariable);
                        ilProcessor.InsertBefore(leaveInstruction, saveResultInstruction);
                        ilProcessor.ReplaceInstructionReferences(leaveInstruction, saveResultInstruction);
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
                    ilProcessor.ReplaceInstructionReferences(instruction, noOpInstruction);
                }
            }
        }

        internal static void ReplaceInstructionReferences(
            this ILProcessor ilProcessor,
            Instruction oldInstruction,
            Instruction newInstruction)
        {
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

            // Update instructions with a target instruction
            foreach (var iteratedInstruction in ilProcessor.Body.Instructions)
            {
                var operand = iteratedInstruction.Operand;

                if (operand == oldInstruction)
                {
                    iteratedInstruction.Operand = newInstruction;
                    continue;
                }
                else if (operand is Instruction[] operands)
                {
                    for (var i = 0; i < operands.Length; ++i)
                    {
                        if (operands[i] == oldInstruction)
                            operands[i] = newInstruction;
                    }
                }
            }
        }
    }
}