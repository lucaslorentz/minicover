namespace Sample.TryFinally
{
    public abstract class AnAbstractClass
    {
        public int AnotherValue { get; }
        private readonly int value = 5;

        protected AnAbstractClass(int anotherValue)
        {
            AnotherValue = anotherValue;
        }
    }
}