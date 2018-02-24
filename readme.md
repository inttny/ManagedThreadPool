//the task will be started immediately by executed the following statement
//because there are 10 threads running default and all threads are idle
	
//if u want to increase/decrease the number of running threads, call the function below:
//ManagedThreadPool.SetRunningThreads(8);
	
//try ManagedThreadPool.IdleThreads[0]
//or  ManagedThreadPool.BusyThreads[0]

inttny.Tools.Threads.Task newTask = new inttny.Tools.Threads.Task();
newTask.Run += () =>
{
	Console.WriteLine("I M wroten by another thread.");
	Thread.Sleep(100);
};
newTask.Finished += () => 
{
	Console.WriteLine("I M wrtoen by another thread too, but defined in main method."); 
};
	
ManagedThreadPool.AppendTask(newTask);
