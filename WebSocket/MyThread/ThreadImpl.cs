using System.Security.Permissions;
using System.Threading;

namespace WebSocket.MyThread
{
    public abstract class ThreadImpl : IThread
    {
        private Thread thread;
        public bool IsRunning { get; private set; }

        public abstract void Run();

        public virtual void Start()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            thread = new Thread(new ThreadStart(Run));
            thread.Start();            
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public virtual void Stop()
        {
            IsRunning = false;

            if (thread.IsAlive)
                thread.Abort();
        }
    }
}
