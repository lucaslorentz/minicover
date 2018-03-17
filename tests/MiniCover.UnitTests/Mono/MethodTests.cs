using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Metadata;
using Xunit;

namespace Mono.Cecil.Tests {

	public class MethodTests : BaseTestFixture {

		[Fact]
		public void AbstractMethod ()
		{
			TestCSharp ("Methods.cs", module => {
				var type = module.Types [1];
				Assert.Equal ("Foo", type.Name);
				Assert.Equal (2, type.Methods.Count);

				var method = type.GetMethod ("Bar");
				Assert.Equal ("Bar", method.Name);
				Assert.True (method.IsAbstract);
				Assert.NotNull (method.ReturnType);

				Assert.Equal (1, method.Parameters.Count);

				var parameter = method.Parameters [0];

				Assert.Equal ("a", parameter.Name);
				Assert.Equal ("System.Int32", parameter.ParameterType.FullName);
			});
		}

		[Fact]
		public void SimplePInvoke ()
		{
			TestCSharp ("Methods.cs", module => {
				var bar = module.GetType ("Bar");
				var pan = bar.GetMethod ("Pan");

				Assert.True (pan.IsPInvokeImpl);
				Assert.NotNull (pan.PInvokeInfo);

				Assert.Equal ("Pan", pan.PInvokeInfo.EntryPoint);
				Assert.NotNull (pan.PInvokeInfo.Module);
				Assert.Equal ("foo.dll", pan.PInvokeInfo.Module.Name);
			});
		}

		[Fact]
		public void GenericMethodDefinition ()
		{
			TestCSharp ("Generics.cs", module => {
				var baz = module.GetType ("Baz");

				var gazonk = baz.GetMethod ("Gazonk");

				Assert.NotNull (gazonk);

				Assert.True (gazonk.HasGenericParameters);
				Assert.Equal (1, gazonk.GenericParameters.Count);
				Assert.Equal ("TBang", gazonk.GenericParameters [0].Name);
			});
		}

		[Fact]
		public void ReturnGenericInstance ()
		{
			TestCSharp ("Generics.cs", module => {
				var bar = module.GetType ("Bar`1");

				var self = bar.GetMethod ("Self");
				Assert.NotNull (self);

				var bar_t = self.ReturnType;

				Assert.True (bar_t.IsGenericInstance);

				var bar_t_instance = (GenericInstanceType) bar_t;

				Assert.Equal (bar.GenericParameters [0], bar_t_instance.GenericArguments [0]);

				var self_str = bar.GetMethod ("SelfString");
				Assert.NotNull (self_str);

				var bar_str = self_str.ReturnType;
				Assert.True (bar_str.IsGenericInstance);

				var bar_str_instance = (GenericInstanceType) bar_str;

				Assert.Equal ("System.String", bar_str_instance.GenericArguments [0].FullName);
			});
		}

		[Fact]
		public void ReturnGenericInstanceWithMethodParameter ()
		{
			TestCSharp ("Generics.cs", module => {
				var baz = module.GetType ("Baz");

				var gazoo = baz.GetMethod ("Gazoo");
				Assert.NotNull (gazoo);

				var bar_bingo = gazoo.ReturnType;

				Assert.True (bar_bingo.IsGenericInstance);

				var bar_bingo_instance = (GenericInstanceType) bar_bingo;

				Assert.Equal (gazoo.GenericParameters [0], bar_bingo_instance.GenericArguments [0]);
			});
		}

		[Fact]
		public void SimpleOverrides ()
		{
			TestCSharp ("Interfaces.cs", module => {
				var ibingo = module.GetType ("IBingo");
				var ibingo_foo = ibingo.GetMethod ("Foo");
				Assert.NotNull (ibingo_foo);

				var ibingo_bar = ibingo.GetMethod ("Bar");
				Assert.NotNull (ibingo_bar);

				var bingo = module.GetType ("Bingo");

				var foo = bingo.GetMethod ("IBingo.Foo");
				Assert.NotNull (foo);

				Assert.True (foo.HasOverrides);
				Assert.Equal (ibingo_foo, foo.Overrides [0]);

				var bar = bingo.GetMethod ("IBingo.Bar");
				Assert.NotNull (bar);

				Assert.True (bar.HasOverrides);
				Assert.Equal (ibingo_bar, bar.Overrides [0]);
			});
		}

		[Fact]
		public void GenericInstanceMethod ()
		{
			TestCSharp ("Generics.cs", module => {
				var type = module.GetType ("It");
				var method = type.GetMethod ("ReadPwow");

				GenericInstanceMethod instance = null;

				foreach (var instruction in method.Body.Instructions) {
					instance = instruction.Operand as GenericInstanceMethod;
					if (instance != null)
						break;
				}

				Assert.NotNull (instance);

				Assert.Equal (TokenType.MethodSpec, instance.MetadataToken.TokenType);
				Assert.False(instance.MetadataToken.RID.Equals(0));
			});
		}

		[Fact]
		public void MethodRefDeclaredOnGenerics ()
		{
			TestCSharp ("Generics.cs", module => {
				var type = module.GetType ("Tamtam");
				var beta = type.GetMethod ("Beta");
				var charlie = type.GetMethod ("Charlie");

				// Note that the test depends on the C# compiler emitting the constructor call instruction as
				// the first instruction of the method body. This requires optimizations to be enabled.
				var new_list_beta = (MethodReference) beta.Body.Instructions [0].Operand;
				var new_list_charlie = (MethodReference) charlie.Body.Instructions [0].Operand;

				Assert.Equal ("System.Collections.Generic.List`1<TBeta>", new_list_beta.DeclaringType.FullName);
				Assert.Equal ("System.Collections.Generic.List`1<TCharlie>", new_list_charlie.DeclaringType.FullName);
			});
		}
	}
}
