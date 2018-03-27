using System;
using System.IO;
using System.Linq;
using Mono.Cecil.Cil;

namespace Mono.Cecil.Tests
{

	public static class Formatter
	{

		public static string FormatInstruction(Instruction instruction)
		{
			var writer = new StringWriter();
			WriteInstruction(writer, instruction);
			return writer.ToString();
		}

		public static string FormatMethodBody(MethodDefinition method)
		{
			var writer = new StringWriter();
			WriteMethodBody(writer, method);
			return writer.ToString();
		}

		public static void WriteMethodBody(TextWriter writer, MethodDefinition method)
		{
			var body = method.Body;

			WriteVariables(writer, body);
			var indentation = 1;
			foreach (Instruction instruction in body.Instructions)
			{
				var handler = body.ExceptionHandlers.FirstOrDefault(a => a.HandlerStart.Equals(instruction));
				if (handler != null)
				{
					writer.Indent(indentation);
					writer.WriteLine("}");
					writer.Indent(indentation);
					writer.WriteLine(FormatHandlerType(handler));
					writer.WriteLine("{");
				}
				if (body.ExceptionHandlers.Any(a => a.HandlerEnd.Equals(instruction)))
				{
					indentation--;
					writer.Indent(indentation);
					writer.WriteLine("}");

				}
			    if (body.ExceptionHandlers.Any(a => a.TryStart.Equals(instruction)))
			    {
			        writer.Indent(indentation);
			        writer.WriteLine(".try");
			        writer.WriteLine("{");
			        indentation++;
			    }
				WriteInstructionLine(writer, body, instruction, indentation);

				
			}
		}

		private static void WriteInstructionLine(TextWriter writer, MethodBody body, Instruction instruction,
			int indentation)
		{
			var sequence_point = body.Method.DebugInformation.GetSequencePoint(instruction);
			if (sequence_point != null)
			{
				writer.WriteLine();
				writer.Indent(indentation);
				WriteSequencePoint(writer, sequence_point);
				writer.WriteLine();
			}

			writer.Indent(indentation);
			WriteInstruction(writer, instruction);
			writer.WriteLine();
		}

		private static void Indent(this TextWriter writer, int numberOfTabulation)
		{
			for (int i = 0; i < numberOfTabulation; i++)
			{
				writer.Write('\t');
			}
		}

		static void WriteVariables(TextWriter writer, MethodBody body)
		{
			var variables = body.Variables;

			writer.Write('\t');
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

		static string GetVariableName(VariableDefinition variable, MethodBody body)
		{
			if (body.Method.DebugInformation.TryGetName(variable, out var name))
				return name;

			return variable.ToString();
		}

		static void WriteInstruction(TextWriter writer, Instruction instruction)
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

		static void WriteSequencePoint(TextWriter writer, SequencePoint sequence_point)
		{
			if (sequence_point.IsHidden)
			{
				writer.Write(".line hidden '{0}'", sequence_point.Document.Url);
				return;
			}
			writer.Write($"// [{sequence_point.StartLine} {sequence_point.EndLine} - {sequence_point.StartColumn} {sequence_point.EndColumn}]");
		}

		static string FormatLabel(int offset)
		{
			string label = "000" + offset.ToString("x");
			return "IL_" + label.Substring(label.Length - 4);
		}

		static string FormatLabel(Instruction instruction)
		{
			return FormatLabel(instruction.Offset);
		}

		static void WriteOperand(TextWriter writer, object operand)
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

		static void WriteLabelList(TextWriter writer, Instruction[] instructions)
		{
			writer.Write("(");

			for (int i = 0; i < instructions.Length; i++)
			{
				if (i != 0) writer.Write(", ");
				writer.Write(FormatLabel(instructions[i].Offset));
			}

			writer.Write(")");
		}

		static string FormatHandlerType(ExceptionHandler handler)
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

		public static string ToInvariantCultureString(object value)
		{
			var convertible = value as IConvertible;
			return (null != convertible)
				? convertible.ToString(System.Globalization.CultureInfo.InvariantCulture)
				: value.ToString();
		}
	}
}
