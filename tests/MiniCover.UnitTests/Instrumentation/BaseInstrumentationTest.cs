using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using MiniCover.HitServices;
using MiniCover.Model;
using MiniCover.UnitTests.TestHelpers;
using Mono.Cecil;
using Mono.Cecil.Tests;
using Xunit;

namespace MiniCover.UnitTests.Instrumentation
{
    public abstract class BaseInstrumentationTest
    {
        private Type _typeToInstrument;
        private MethodBase _methodToInstrument;

        public BaseInstrumentationTest(Type typeToInstrument)
        {
            _typeToInstrument = typeToInstrument;
        }

        public BaseInstrumentationTest(MethodBase methodToInstrument)
        {
            _methodToInstrument = methodToInstrument;
        }

        [Fact]
        public void Test()
        {
            InstrumentedAssembly instrumentedAssembly;
            TypeDefinition typeDefinition;

            if (_methodToInstrument != null)
            {
                var methodDefinition = _methodToInstrument.ToDefinition();
                typeDefinition = methodDefinition?.DeclaringType;
                instrumentedAssembly = methodDefinition.Instrument();
                var il = new ILFormatter(false).FormatMethodBody(methodDefinition);
                il.Should().Be(ExpectedIL);
            }
            else
            {
                typeDefinition = _typeToInstrument.ToDefinition();
                instrumentedAssembly = typeDefinition.Instrument();
                var il = new ILFormatter(false).FormatType(typeDefinition);
                il.Should().Be(ExpectedIL);
            }

            var instrumentedInstructions = instrumentedAssembly.SourceFiles
                .SelectMany(kv => kv.Value.Instructions)
                .ToArray();

            if (ExpectedInstructions != null)
                instrumentedInstructions.Should().BeEquivalentTo(ExpectedInstructions);

            var instrumentedType = typeDefinition.Load();

            var instrumentedTestType = instrumentedType.Assembly.GetType(GetType().FullName);
            var functionalTestMethod = instrumentedTestType.GetMethod(nameof(FunctionalTest));

            HitContext.Current = new HitContext("Assembly", "Class", "Method");
            var instrumentedTest = Activator.CreateInstance(instrumentedTestType);
            functionalTestMethod.Invoke(instrumentedTest, new object[0]);
            HitContext.Current.Hits.Should().BeEquivalentTo(ExpectedHits);
        }

        public abstract string ExpectedIL { get; }
        public abstract IDictionary<int, int> ExpectedHits { get; }
        public virtual InstrumentedInstruction[] ExpectedInstructions => null;

        public abstract void FunctionalTest();
    }
}
