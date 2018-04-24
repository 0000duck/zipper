using System;
using System.Collections.Generic;
using System.Threading;

namespace zipper
{
    public class ConcurrentQueue<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private bool _completed;

        public void Push(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (_completed)
                return;

            lock (_queue)
            {
                if (_completed)
                    return;

                _queue.Enqueue(obj);
                Monitor.Pulse(_queue);
            }
        }

        public T Pop()
        {
            if (Completed())
                return default(T);

            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_queue);
                    if (_completed)
                        return default(T);
                }
                return _queue.Dequeue();
            }
        }

        public void Complete()
        {
            if (_completed)
                return;

            lock (_queue)
            {
                if (_completed)
                    return;
                _completed = true;
                Monitor.PulseAll(_queue);
            }
        }

        public bool Completed()
        {
            return _completed && _queue.Count == 0;
        }
    }
}