using System;
using System.Collections.Generic;
using System.Threading;

namespace zipper
{
    public class ThreadWorker
    {
        private readonly List<Thread> _threads;

        public ThreadWorker()
        {
            _threads = new List<Thread>();
        }

        public void StartThreads()
        {
            _threads.ForEach(t => { t.IsBackground = true; t.Start(); });
        }
        
        public void WaitAll()
        {
            _threads.ForEach(i => i.Join());
        }

        public void CreateProcessThreads(List<Action> delegateActionsProcess, List<Action> delegateActionsAbort, int countOfThreads)
        {
            if (delegateActionsProcess == null) throw new ArgumentNullException(nameof(delegateActionsProcess));
            if (delegateActionsAbort == null) throw new ArgumentNullException(nameof(delegateActionsAbort));
            if (delegateActionsProcess.Count == 0)
                throw new ArgumentException("Argument is empty collection", nameof(delegateActionsProcess));
            if (countOfThreads <= 0) throw new ArgumentOutOfRangeException(nameof(countOfThreads));

            for (var i = 0; i < countOfThreads; i++)
            {
                CreateProcessThread(delegateActionsProcess, delegateActionsAbort);
            }
        }

        private void CreateProcessThread(List<Action> delegateActionsProcess, List<Action> delegateActionsAbort)
        {
            if (delegateActionsProcess == null) throw new ArgumentNullException(nameof(delegateActionsProcess));
            if (delegateActionsAbort == null) throw new ArgumentNullException(nameof(delegateActionsAbort));
            if (delegateActionsProcess.Count == 0)
                throw new ArgumentException("Argument is empty collection", nameof(delegateActionsProcess));

            _threads.Add(new Thread(() =>
            {
                try
                {
                    delegateActionsProcess.ForEach(a => a.Invoke());
                }
                catch (Exception ex)
                {
                    delegateActionsAbort.ForEach(a => a.Invoke());
                    throw ex;
                }
            }));
        }
    }
}