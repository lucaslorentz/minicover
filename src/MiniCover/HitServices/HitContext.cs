using System.Threading;

namespace MiniCover.HitServices
{
    public class HitContext
    {
        private static readonly AsyncLocal<TestMethodInfo> AsyncLocalTrace = new AsyncLocal<TestMethodInfo>();

        public static TestMethodInfo Get()
        {
            return AsyncLocalTrace.Value;
        }

        public static void Set(TestMethodInfo trace)
        {
            AsyncLocalTrace.Value = trace;
        }
    }
}