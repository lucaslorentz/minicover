namespace Sample.TryFinally
{
    public sealed class HeritingClass : AnAbstractClass
    {
        public HeritingClass(int anotherValue) 
            : base(anotherValue)
        {
            this.Value = 15 * anotherValue;
        }

        public AnotherClass Instance { get; } = new AnotherClass();
        public int Value { get; }
    }
}