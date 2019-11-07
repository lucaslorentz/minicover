using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono.Cecil.Cil
{
    public static class ILProcessorExtensions
    {
        public static Instruction EncapsulateWithTryFinally(this ILProcessor ilProcessor)
        {
            var body = ilProcessor.Body;

            var firstInstruction = body.Instructions[0];

            var newReturn = ilProcessor.ReplaceReturnsByLeave();

            var endFinally = Instruction.Create(OpCodes.Endfinally);
            ilProcessor.InsertBefore(newReturn, endFinally);

            if (body.Instructions[0].Equals(firstInstruction))
            {
                Instruction tryStart = Instruction.Create(OpCodes.Nop);
                ilProcessor.InsertBefore(firstInstruction, tryStart);
            }

            Instruction finallyStart = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(endFinally, finallyStart);

            var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = firstInstruction,
                TryEnd = finallyStart,
                HandlerStart = finallyStart,
                HandlerEnd = newReturn,
            };

            body.ExceptionHandlers.Add(handler);

            return endFinally;
        }

        public static Instruction ReplaceReturnsByLeave(this ILProcessor ilProcessor)
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
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = newReturnInstruction;
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
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = loadResultInstruction;

                        ilProcessor.InsertBefore(instruction, new[] {
                            ilProcessor.Create(OpCodes.Stloc, returnVariable)
                        }, true);
                    }
                }

                return loadResultInstruction;
            }
        }

        public static void RemoveTailInstructions(this ILProcessor ilProcessor)
        {
            foreach (var instruction in ilProcessor.Body.Instructions.ToArray())
            {
                if (instruction.OpCode == OpCodes.Tail)
                {
                    instruction.OpCode = OpCodes.Nop;
                    instruction.Operand = null;
                }
            }
        }

        public static void ReplaceInstructionReferences(
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

        public static Instruction InsertBefore(
            this ILProcessor ilProcessor,
            Instruction target,
            IEnumerable<Instruction> newInstructions,
            bool updateReferences)
        {
            var newTarget = target;

            foreach (var instruction in newInstructions.Reverse())
            {
                ilProcessor.InsertBefore(newTarget, instruction);
                newTarget = instruction;
            }

            if (updateReferences)
                ilProcessor.ReplaceInstructionReferences(target, newTarget);

            return newTarget;
        }
    }
}