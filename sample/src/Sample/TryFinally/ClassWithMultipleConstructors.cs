namespace Sample.TryFinally
{
    public class ClassWithMultipleConstructors
    {
        private static readonly int staticValue;
        private readonly int value;
        private ClassWithMultipleConstructors() : this(15)
        {
        }

        private ClassWithMultipleConstructors(int value)
        {
            this.value = value * staticValue;
        }

        static ClassWithMultipleConstructors()
        {
            staticValue = 15;
        }

        public static ClassWithMultipleConstructors BuildFor(int value) => new ClassWithMultipleConstructors(value);

        public static ClassWithMultipleConstructors Default() => new ClassWithMultipleConstructors();
    }

    public class AnotherClassWithMultipleConstructors
    {
        public string Text { get; }
        private readonly int value;

        public AnotherClassWithMultipleConstructors(int value, string text)
        {
            Text = text;
            this.value = value;
        }

        public AnotherClassWithMultipleConstructors() 
            : this("test")
        {
        }

        public AnotherClassWithMultipleConstructors(string text)
        : this(12, text)
        {
            
        }
    }
}