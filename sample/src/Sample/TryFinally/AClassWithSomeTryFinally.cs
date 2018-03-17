namespace Sample.TryFinally
{
    public class AClassWithSomeTryFinally
    {
        public int MultiplyByTwo(int value)
        {
            var test = new AClassWithATryFinallyInConstructor();
            try
            {
                return value * 2;
            }
            finally
            {
                test.Exit();
            }
        }
    }
}