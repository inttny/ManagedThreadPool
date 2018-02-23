using System;
using System.Threading;

namespace inttny.Tools
{
    public class WorkThread
    {
        public Guid ThreadNum = Guid.NewGuid();
        /// <summary>
        /// 指示该线程是否正在关闭
        /// true - 指示当运行完当前任务后触发FinishTask事件应该将此线程关闭
        /// </summary>
        public bool Closing { get; private set; }
        Thread InnerThread;
        Task RunningTask = null;
        //初始化为没信号，空闲线程时阻塞线程以免消耗CPU资源
        ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false);
        public event Action FinishTask;
        public WorkThread()
        {
            InnerThread = new Thread(Run);
            InnerThread.Start();
            FinishTask += () => { };
        }
        void Run()
        {
            while (!Closing)
            {
                manualResetEvent.Wait();
                //开始执行线程
                if (RunningTask != null)
                {
                    RunningTask.Start();
                    RunningTask = null;
                    FinishTask();
                }
                manualResetEvent.Reset();
            }
        }
        /// <summary>
        /// 开始新任务，当前线程若有正在运行的任务或者当前线程正在关闭则失败，返回false
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        internal bool Start(Task newTask)
        {
            if (Closing || RunningTask != null) return false;

            RunningTask = newTask;
            manualResetEvent.Set();
            return true;
        }
        /// <summary>
        /// 等待任务完成后关闭当前线程，该操作不可逆。
        /// </summary>
        internal void Close()
        {
            Closing = false;
            manualResetEvent.Set();
        }
    }
}
