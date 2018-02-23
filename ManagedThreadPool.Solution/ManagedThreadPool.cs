using System.Collections.Generic;

namespace inttny.Tools
{
    public static class ManagedThreadPool
    {
        readonly static object LockObj = new object();
        public static List<WorkThread> IdleThreads = new List<WorkThread>(10);
        public static List<WorkThread> BusyThreads = new List<WorkThread>(10);
        static Queue<Task> TaskQueue = new Queue<Task>(100);
        /// <summary>
        /// 可管理的线程池，默认运行线程数为10。可调用SetRunningThreads 调整运行线程数量
        /// </summary>
        static ManagedThreadPool() => SetRunningThreads(10);
        static void AppendThread()
        {
            var newThread = new WorkThread();
            newThread.FinishTask += () => { ChangeThreadStatus(newThread); };
            IdleThreads.Add(newThread);
        }
        static void ChangeThreadStatus(WorkThread targetThread)
        {
            if (targetThread.Closing)//指示该线程需要关闭
            {
                lock (LockObj) BusyThreads.Remove(targetThread);
                targetThread.Close();
                return;
            }

            Task nextTask;
            lock (LockObj) { nextTask = TaskQueue.Dequeue(); }
            if (nextTask == null)//队列已空，将线程从忙碌列表移至空闲列表
            {
                lock (LockObj)
                {
                    BusyThreads.Remove(targetThread);
                    IdleThreads.Add(targetThread);
                }
            }
            else targetThread.Start(nextTask);
        }

        /// <summary>
        /// 设置当前运行线程数量。空闲线程即时关闭，忙碌线程将在完成当前任务后关闭。
        /// </summary>
        /// <param name="threadsCount"></param>
        public static void SetRunningThreads(int threadsCount)
        {
            lock (LockObj)
            {
                int runningThreadsCount = BusyThreads.Count + IdleThreads.Count;
                if (runningThreadsCount == threadsCount) return;
                else if (runningThreadsCount < threadsCount)//添加运行的线程数
                    for (int i = threadsCount - runningThreadsCount; i-- > 0;)
                    { AppendThread(); }
                else//减少运行的线程数
                {
                    for (int i = runningThreadsCount - threadsCount; i-- > 0;)
                    {
                        WorkThread closingThread;
                        int indexOfThread = -1;
                        //先从空闲线程列表中取出线程
                        if (IdleThreads.Count > 0)
                        {
                            indexOfThread = IdleThreads.Count - 1;
                            closingThread = IdleThreads[indexOfThread];
                            IdleThreads.RemoveAt(indexOfThread);
                        }
                        else //从忙碌线程列表中取出线程
                        {
                            indexOfThread = BusyThreads.Count;
                            do { closingThread = BusyThreads[--indexOfThread]; }
                            while (closingThread.Closing);//如果当前取到的线程已经是关闭中状态，则取前一个线程，直到取到非关闭中状态的线程
                        }

                        closingThread.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 将新任务加入任务列表。当有空闲线程时立即开始执行新任务
        /// </summary>
        /// <param name="newTask"></param>
        public static void AppendTask(Task newTask)
        {
            lock (LockObj)
            {
                if (IdleThreads.Count > 0)//如果空闲列表非空，则直接从空闲列表中取出空闲线程进行使用
                {
                    int indexOfThread = IdleThreads.Count - 1;
                    WorkThread targetThread = IdleThreads[indexOfThread];
                    IdleThreads.RemoveAt(indexOfThread);//从空闲列表删目标线程
                    BusyThreads.Add(targetThread);//将目标线程加入忙碌列表
                    targetThread.Start(newTask);
                }
                else TaskQueue.Enqueue(newTask);//如果空闲列表为空，则将Task加入任务队列等待线程空闲后执行
            }
        }
    }
}
