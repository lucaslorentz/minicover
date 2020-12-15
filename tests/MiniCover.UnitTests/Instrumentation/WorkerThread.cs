using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class WorkerThread : BaseTest
    {
        public static class Class
        {
            public static Task RunWorkerThread()
            {
                var tcs = new TaskCompletionSource<bool>();
                var thread = new Thread(() => DoWork(tcs));
                thread.Start();
                return tcs.Task;
            }

            private static void DoWork(TaskCompletionSource<bool> tcs)
            {
                Thread.Sleep(100);
                tcs.SetResult(true);
            }
        }

        public WorkerThread() : base(typeof(Class))
        {
        }

        public override void FunctionalTest()
        {
            Class.RunWorkerThread().GetAwaiter().GetResult();
        }

        public override int? ExpectedHitCount => 7;
    }
}
