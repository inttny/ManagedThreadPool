using inttny.Tools.Threads;

namespace inttny.Tools.Threads.Test
{
    public class UsageDemo
    {
        void AnythingWannaBeAsync()
        {
            Console.WriteLine("I M wroten by another thread.");
            Thread.Sleep(100);
        }
        
        void Main()
        {
            inttny.Tools.Threads.Task newTask = new inttny.Tools.Threads.Task();
            newTask.Run += AnythingWannaBeAsync;
            newTask.Finished += () => { Console.WriteLine("I M wrtoen by another thread too, but defined in main method."); }
            ManagedThreadPool.AppendTask(newTask);
            
            //now the task started immediately, because there are 10 threads running default and all threads are idle
            
            //if u want to increase/decrease the number of running threads, call the function below:
            //ManagedThreadPool.SetRunningThreads(8);
            
            //try ManagedThreadPool.IdleThreads[0]
            //or  ManagedThreadPool.BusyThreads[0]
       }
    }
}
