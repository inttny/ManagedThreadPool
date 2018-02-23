using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace inttny.Tools.Threads.Tests
{
    [TestClass]
    public class Test2AppendTask
    {
        [ClassInitialize]
        public static void Startup(TestContext context) => ManagedThreadPool.SetRunningThreads(10);

        [TestMethod]
        public void ModifyRunningThreadsTest()
        {
            ManagedThreadPool.SetRunningThreads(5);
            Assert.AreEqual(5, ManagedThreadPool.IdleThreads.Count);
            Assert.AreEqual(0, ManagedThreadPool.BusyThreads.Count);

            ManagedThreadPool.SetRunningThreads(15);
            Assert.AreEqual(15, ManagedThreadPool.IdleThreads.Count);
            Assert.AreEqual(0, ManagedThreadPool.BusyThreads.Count);
        }
        
        [TestMethod]
        public void AppendTaskTest()
        {
            int res = 0;

            Task testTask = new Task();
            testTask.Run += () => { res++; };
            testTask.Finished += () => { Assert.AreEqual(1, res); };

            ManagedThreadPool.AppendTask(testTask);
            Thread.Sleep(100);
            Assert.IsTrue(testTask.IsCompleted);
        }
    }
}
