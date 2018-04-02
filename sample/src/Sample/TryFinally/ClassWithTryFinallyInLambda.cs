using System;
using System.Linq;

namespace Sample.TryFinally
{
    public class ClassWithTryFinallyInLambda
    {
        public int Add2ToEachValueAndSumThemWithConsoleWrite(params int[] values)
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
                        Console.WriteLine("end");
                    }
                }).Sum();
            }
            finally
            {
                Console.WriteLine("end");
                
            }
            
        }
    }
}