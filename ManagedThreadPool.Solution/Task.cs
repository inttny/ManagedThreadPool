using System;
using System.Threading;

namespace inttny.Tools
{
    public class Task : IAsyncResult
    {
        //暂时没有用到异步状态信息，目前没有想到什么情景下会使用到它。
        public object AsyncState { get; private set; }

        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        public WaitHandle AsyncWaitHandle => manualResetEvent;

        public bool CompletedSynchronously => false;//这是个什么鬼属性？直接pass

        bool _isComplete = false;
        public bool IsCompleted => _isComplete;

        public event Action Finished;
        public event Action Run;

        public Task()
        {
            Run += () => { _isComplete = false; };
            Finished += () => { _isComplete = true; };
        }
        internal void Start()
        {
            Run();
            Finished();
        }
    }
}
