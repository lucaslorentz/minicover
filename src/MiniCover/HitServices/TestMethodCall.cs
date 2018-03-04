using System.Reflection;

namespace MiniCover.HitServices
{
    public sealed class TestMethodCall
    {
        private TestMethodCall(MethodBase methodBase)
        {
            this.MethodBase = methodBase;
        }

        public MethodBase MethodBase { get; }

        public static TestMethodCall Current
        {
            get => HitContext.Get();
            set
            {
                if (value == null)
                {
                    HitContext.Clear();
                }
                else
                {
                    HitContext.Set(value);
                }
            }
        }

        public static TestMethodCall Create(MethodBase methodBase)
        {
            return new TestMethodCall(methodBase);
        }
    }
}