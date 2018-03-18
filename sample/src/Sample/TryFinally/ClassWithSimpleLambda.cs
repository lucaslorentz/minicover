using System.Linq;

namespace Sample.TryFinally
{
    public class ClassWithSimpleLambda
    {
        public int Add2ToEachValueAndSumThem(params int[] values)
        {
            return values.Select(a => a + 2).Sum();
        }
    }
}