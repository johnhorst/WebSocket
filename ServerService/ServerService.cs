using System.Threading;
using System.ServiceProcess;
using WebSocket.Logger;
using WebSocket.Network;

namespace ServerService
{
    public partial class ServerService : ServiceBase
    {
        private ServerManager sm;
        private ManualResetEvent mre;

        public ServerService()
        {
            InitializeComponent();
            mre = new ManualResetEvent(false);
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length == 2)
                sm = new ServerManager(args[0], int.Parse(args[1]));
            else
                sm = new ServerManager("127.0.0.1", 2024);
            sm.Start();
            sm.ClientManager.OnUserCountChange += ClientManager_OnUserCountChange;
        }

        private void ClientManager_OnUserCountChange(object sender, int userOnline)
        {
            DebugLogger.AddLog("User online:" + userOnline);
        }

        protected override void OnStop()
        {

            sm.Stop();
            DebugLogger.SaveLog("logger.txt");
        }
    }
}
