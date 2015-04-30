using System;
using System.Security.Permissions;
using System.Threading;

namespace WebSocket.MyThread
{
    public abstract class ThreadImpl : IThread
    {
        private Thread thread;
        private AutoResetEvent are = new AutoResetEvent(false);

        public bool IsRunning { get; private set; } = false;
        public bool IsSleepMode { get; private set; } = false;  

        public abstract void Run();

        public virtual void Start()
        {            
            if (IsRunning)
                return;
            IsRunning = true;
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        public virtual void Stop()
        {
            Stop(false);
        }

        public void SleepMode()
        {
            if (!IsSleepMode)
            {
                IsSleepMode = true;
                are.WaitOne();
            }
        }

        public void WakeUp()
        {
            if(IsSleepMode)
            {
                IsSleepMode = false;
                are.Set();
            }
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public virtual void Stop(bool forceStop)
        {
            if (IsSleepMode)
                are.Set();
            IsRunning = false;
            if (!thread.Join(3000) && forceStop)
                thread.Abort();
        }
    }
}
