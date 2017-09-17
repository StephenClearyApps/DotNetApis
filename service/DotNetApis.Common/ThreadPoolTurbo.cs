using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetApis.Common
{
    public static class ThreadPoolTurbo
    {
        public static IDisposable Engage(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism <= 0)
                return null;

            ThreadPool.GetMinThreads(out var workerThreads, out var ioThreads);
            ThreadPool.SetMinThreads(workerThreads + maxDegreeOfParallelism, ioThreads);
            return new RestoreThreadPoolMinThreads(workerThreads, ioThreads);
        }

        private sealed class RestoreThreadPoolMinThreads : IDisposable
        {
            private readonly int _workerThreads;
            private readonly int _ioThreads;

            public RestoreThreadPoolMinThreads(int workerThreads, int ioThreads)
            {
                _workerThreads = workerThreads;
                _ioThreads = ioThreads;
            }

            public void Dispose() => ThreadPool.SetMinThreads(_workerThreads, _ioThreads);
        }
    }
}
