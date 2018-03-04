using System.Threading;

namespace MiniCover.HitServices
{
    public class HitContext
    {
        private static readonly AsyncLocal<TestMethodCall> AsyncLocalTrace = new AsyncLocal<TestMethodCall>();

        public static TestMethodCall Get()
        {
            return AsyncLocalTrace.Value;
        }

        public static void Set(TestMethodCall trace)
        {
            AsyncLocalTrace.Value = trace;
        }

        public static void Clear()
        {
            AsyncLocalTrace.Value = null;
        }
    }
}