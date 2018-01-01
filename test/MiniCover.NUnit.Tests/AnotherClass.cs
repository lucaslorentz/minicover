namespace MiniCover.Tests.NUnit
{
    public class AnotherClass
    {
        private int _someProperty = 7;

        public int SomeProperty
        {
            get
            {
                return _someProperty;
            }
            set
            {
                _someProperty = value;
            }
        }

        public int SomeMethod()
        {
            return SomeProperty * 2;
        }

        public void AnotherMethod()
        {
            SomeProperty = SomeProperty * 2;
        }
    }
}
