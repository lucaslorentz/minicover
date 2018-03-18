using System;
using System.IO;
using System.Linq;
using SR = System.Reflection;

namespace Mono.Cecil.Tests
{

	public static class Extensions
	{

		public static MethodDefinition GetMethod(this TypeDefinition self, string name)
		{
			return self.Methods.First(m => m.Name == name);
		}

		public static FieldDefinition GetField(this TypeDefinition self, string name)
		{
			return self.Fields.First(f => f.Name == name);
		}

		public static TypeDefinition ToDefinition(this Type self)
		{
			var module = ModuleDefinition.ReadModule(new MemoryStream(File.ReadAllBytes(self.Module.FullyQualifiedName)));
			return (TypeDefinition)module.LookupToken(self.MetadataToken);
		}

		public static MethodDefinition ToDefinition(this SR.MethodBase method)
		{
			var declaring_type = method.DeclaringType.ToDefinition();
			return (MethodDefinition)declaring_type.Module.LookupToken(method.MetadataToken);
		}

		public static FieldDefinition ToDefinition(this SR.FieldInfo field)
		{
			var declaring_type = field.DeclaringType.ToDefinition();
			return (FieldDefinition)declaring_type.Module.LookupToken(field.MetadataToken);
		}

		public static TypeReference MakeGenericType(this TypeReference self, params TypeReference[] arguments)
		{
			if (self.GenericParameters.Count != arguments.Length)
				throw new ArgumentException();

			var instance = new GenericInstanceType(self);
			foreach (var argument in arguments)
				instance.GenericArguments.Add(argument);

			return instance;
		}

		public static bool IsVarArg(this IMethodSignature self)
		{
			return (self.CallingConvention & MethodCallingConvention.VarArg) != 0;
		}

		public static int GetSentinelPosition(this IMethodSignature self)
		{
			if (!self.HasParameters)
				return -1;

			var parameters = self.Parameters;
			for (int i = 0; i < parameters.Count; i++)
				if (parameters[i].ParameterType.IsSentinel)
					return i;

			return -1;
		}

		public static MethodReference MakeGenericMethod(this MethodReference self, params TypeReference[] arguments)
		{
			if (self.GenericParameters.Count != arguments.Length)
				throw new ArgumentException();

			var instance = new GenericInstanceMethod(self);
			foreach (var argument in arguments)
				instance.GenericArguments.Add(argument);

			return instance;
		}
	}
}
