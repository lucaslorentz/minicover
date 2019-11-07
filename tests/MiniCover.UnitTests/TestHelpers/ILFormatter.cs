using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Mono.Cecil.Cil;

namespace Mono.Cecil.Tests
{
    public class ILFormatter
    {
        public bool IncludeSequencePoints { get; set; }

        public ILFormatter(bool includeSequencePoints)
        {
            IncludeSequencePoints = includeSequencePoints;
        }

        public string FormatInstruction(Instruction instruction)
        {
            var writer = new StringWriter();
            WriteInstruction(writer, instruction);
            return writer.ToString();
        }

        public string FormatType(TypeDefinition type)
        {
            var writer = new StringWriter();
            var indentedWriter = new IndentedTextWriter(writer);
            WriteType(indentedWriter, type);
            return writer.ToString();
        }

        public string FormatMethodBody(MethodDefinition method)
        {
            var writer = new StringWriter();
            var indentedWriter = new IndentedTextWriter(writer);
            WriteMethodBody(indentedWriter, method.Body);
            return writer.ToString();
        }

        public void WriteType(IndentedTextWriter writer, TypeDefinition type)
        {
            writer.Write(".class ");
            if (type.IsPublic)
                writer.Write("public ");
            writer.WriteLine(type.Name);
            writer.WriteLine("{");
            writer.Indent++;
            foreach (var method in type.Methods)
                WriteMethod(writer, method);
            foreach (var nestedType in type.NestedTypes)
                WriteType(writer, nestedType);
            writer.Indent--;
            writer.WriteLine("}");
        }

        public void WriteMethod(IndentedTextWriter writer, MethodDefinition method)
        {
            writer.Write(".method ");
            if (method.IsPublic)
                writer.Write("public ");
            if (method.IsStatic)
                writer.Write("static ");
            writer.Write($"{method.ReturnType.FullName} ");
            writer.WriteLine(method.Name);
            writer.WriteLine("{");
            writer.Indent++;
            WriteMethodBody(writer, method.Body);
            writer.Indent--;
            writer.WriteLine("}");
        }

        public void WriteMethodBody(IndentedTextWriter writer, MethodBody body)
        {
            WriteVariables(writer, body);

            foreach (Instruction instruction in body.Instructions)
            {
                foreach (var tryStart in body.ExceptionHandlers.Where(a => a.TryStart.Equals(instruction)))
                {
                    writer.WriteLine(".try");
                    writer.WriteLine("{");
                    writer.Indent++;
                }
                foreach (var tryEnd in body.ExceptionHandlers.Where(a => a.TryEnd.Equals(instruction)))
                {
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                foreach (var handlerStart in body.ExceptionHandlers.Where(a => a.HandlerStart.Equals(instruction)))
                {
                    writer.WriteLine(FormatHandlerType(handlerStart));
                    writer.WriteLine("{");
                    writer.Indent++;
                }
                foreach (var handlerEnd in body.ExceptionHandlers.Where(a => a.HandlerEnd.Equals(instruction)))
                {
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                WriteInstructionLine(writer, body, instruction);
            }
        }

        private void WriteInstructionLine(IndentedTextWriter writer, MethodBody body, Instruction instruction)
        {
            if (IncludeSequencePoints)
            {
                var sequence_point = body.Method.DebugInformation.GetSequencePoint(instruction);
                if (sequence_point != null)
                {
                    WriteSequencePoint(writer, sequence_point);
                    writer.WriteLine();
                }
            }
            WriteInstruction(writer, instruction);
            writer.WriteLine();
        }

        void WriteVariables(TextWriter writer, MethodBody body)
        {
            var variables = body.Variables;

            writer.Write(".locals {0}(", body.InitLocals ? "init " : string.Empty);

            for (int i = 0; i < variables.Count; i++)
            {
                if (i > 0)
                    writer.Write(", ");

                var variable = variables[i];

                writer.Write("{0} {1}", variable.VariableType, GetVariableName(variable, body));
            }
            writer.WriteLine(")");
        }

        string GetVariableName(VariableDefinition variable, MethodBody body)
        {
            if (body.Method.DebugInformation.TryGetName(variable, out var name))
                return name;

            return variable.ToString();
        }

        void WriteInstruction(TextWriter writer, Instruction instruction)
        {
            writer.Write(FormatLabel(instruction.Offset));
            writer.Write(": ");
            writer.Write(instruction.OpCode.Name);
            if (null != instruction.Operand)
            {
                writer.Write(' ');
                WriteOperand(writer, instruction.Operand);
            }

            if (instruction.OpCode == OpCodes.Ldarg_0)
            {
                writer.Write(" // this");
            }
        }

        void WriteSequencePoint(TextWriter writer, SequencePoint sequence_point)
        {
            if (sequence_point.IsHidden)
            {
                writer.Write(".line hidden '{0}'", sequence_point.Document.Url);
                return;
            }
            writer.Write($"// [{sequence_point.StartLine} {sequence_point.EndLine} - {sequence_point.StartColumn} {sequence_point.EndColumn}]");
        }

        string FormatLabel(int offset)
        {
            string label = "000" + offset.ToString("x");
            return "IL_" + label.Substring(label.Length - 4);
        }

        void WriteOperand(TextWriter writer, object operand)
        {
            if (null == operand) throw new ArgumentNullException(nameof(operand));

            var target = operand as Instruction;
            if (null != target)
            {
                writer.Write(FormatLabel(target.Offset));
                return;
            }

            var targets = operand as Instruction[];
            if (null != targets)
            {
                WriteLabelList(writer, targets);
                return;
            }

            string s = operand as string;
            if (null != s)
            {
                writer.Write("\"" + s + "\"");
                return;
            }

            var parameter = operand as ParameterDefinition;
            if (parameter != null)
            {
                writer.Write(ToInvariantCultureString(parameter.Sequence));
                return;
            }

            s = ToInvariantCultureString(operand);
            writer.Write(s);
        }

        void WriteLabelList(TextWriter writer, Instruction[] instructions)
        {
            writer.Write("(");

            for (int i = 0; i < instructions.Length; i++)
            {
                if (i != 0) writer.Write(", ");
                writer.Write(FormatLabel(instructions[i].Offset));
            }

            writer.Write(")");
        }

        string FormatHandlerType(ExceptionHandler handler)
        {
            var handler_type = handler.HandlerType;
            var type = handler_type.ToString().ToLowerInvariant();

            switch (handler_type)
            {
                case ExceptionHandlerType.Catch:
                    return string.Format("{0} {1}", type, handler.CatchType.FullName);
                case ExceptionHandlerType.Filter:
                    throw new NotImplementedException();
                default:
                    return type;
            }
        }

        public string ToInvariantCultureString(object value)
        {
            var convertible = value as IConvertible;
            return (null != convertible)
                ? convertible.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : value.ToString();
        }
    }
}
