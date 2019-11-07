using System;
using System.Linq;

namespace Sample.TryFinally
{
    public class ClassWithTryFinallyInLambda
    {
        public int Add2ToEachValueAndSumThem(params int[] values)
        {
            try
            {
                return values.Select(a =>
                {
                    try
                    {
                        return a + 2;
                    }
                    finally
                    {
                        SomeMethod();
                    }
                }).Sum();
            }
            finally
            {
                SomeMethod();
            }
        }

        public void SomeMethod() {

        }
    }
}