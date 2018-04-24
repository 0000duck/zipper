using System;
using System.Threading;
using Xunit;

namespace zipperTests
{
    public class ConcurrentQueueTests
    {
        public class ConcurrentQueueMonitorTests
        {
            private static readonly zipper.ConcurrentQueue<string> Queue = new zipper.ConcurrentQueue<string>();
            private const int QueueIterations = 10;

            [Fact]
            public void IntegrationTest_Success()
            {
                var popIterations = 0;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    for (int i = 0; i < QueueIterations; i++)
                        Queue.Push(Guid.NewGuid().ToString());
                }, null);

                ThreadPool.QueueUserWorkItem(delegate
                {
                    while (Queue.Pop() != null)
                    {
                        popIterations++;
                    }
                }, null);
                Thread.Sleep(5000);
                Queue.Complete();

                Assert.Equal(QueueIterations, popIterations);
            }

            [Fact]
            public void Push_Null_ArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => Queue.Push(null));
            }
        }
    }
}
