using System.Net.Sockets;
using WebSocket.Logger;

namespace WebSocket.Network
{
    public class ServerManager
    {
        private Server server;
        private ClientManager cm;

        public ServerManager(string ip,int port)
        {
            server = new Server(ip, port);
            cm = new ClientManager();
            server.OnConnect += Server_OnConnect;
        }        

        public Server Server { get { return server; } }
        public ClientManager ClientManager { get { return cm; } }

        public void Start()
        {
            Server.Start();
            ClientManager.Start();
        }

        public void Stop()
        {
            ClientManager.Stop();
            Server.Stop();            
        }

        private void Server_OnConnect(object sender,TcpClient client)
        {
            try
            {
                WebSocket c = new WebSocket(client);
                ClientManager.AddClient(c); 
            }
            catch(WebSocketException ex)
            {
                DebugLogger.AddLog("Exception:" + ex.Message);
            }           
        }
    }
}
