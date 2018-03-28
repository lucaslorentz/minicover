using System.Threading.Tasks;

namespace Sample
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
            for (var i = 0; i < 50000; i++)
            {
                SomeMethod();
            }
        }

        public Task AMethodAsync()
        {
            return Task.FromResult(1);
        }

        public void AMethodNotAsync()
        {
            SomeProperty++;
        }

        public dynamic AMethodWithMultipleReturn(int value)
        {
            if (value % 2 == 0)
                return 2;
            if (value % 3 == 0) return "3";
            return 1;
        }
    }
}
