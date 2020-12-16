using System;
using System.Linq;
using System.Threading.Tasks;
using Sample.TryFinally;
using Xunit;

namespace Sample.UnitTests
{
    public class WorkerThreadTests
    {
        [Fact]
        public async Task TestWorkerThread()
        {
            await WorkerThread.RunWorkerThread();
        }
    }
}
