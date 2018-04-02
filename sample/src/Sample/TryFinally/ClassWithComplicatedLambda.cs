using System;
using System.Linq;

namespace Sample.TryFinally
{
    public class ClassWithComplicatedLambda
    {
        public int Add2ToEachValueAndSumThemWithConsoleWrite(params int[] values)
        {
            return values.Select(a =>
            {
                var value = a + 2;
                Console.WriteLine(value);
                return value;
            }).Sum();
        }
    }
}