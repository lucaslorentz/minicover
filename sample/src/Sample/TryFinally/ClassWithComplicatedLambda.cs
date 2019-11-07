using System;
using System.Linq;

namespace Sample.TryFinally
{
    public class ClassWithComplicatedLambda
    {
        public int Add2ToEachValueAndSumThem(params int[] values)
        {
            return values.Select(a =>
            {
                var value = a + 2;
                return value;
            }).Sum();
        }
    }
}