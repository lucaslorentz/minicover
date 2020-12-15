using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    public static class WorkerThread
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
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(100);
            }

            tcs.SetResult(true);
        }
    }
}
