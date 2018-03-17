namespace Sample.TryFinally
{
    public class AClassWithATryFinallyInConstructor
    {
        private readonly int value;
        public AClassWithATryFinallyInConstructor()
        {
            try
            {
                value = 5;
            }
            finally
            {
                Exit();
            }
        }

        public void Exit()
        {
        }
    }
}