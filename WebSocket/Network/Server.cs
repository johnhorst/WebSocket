using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WebSocket.Logger;
using WebSocket.MyThread;

namespace WebSocket.Network
{
    public class Server : ThreadImpl
    {
        private TcpListener server;
        private AutoResetEvent are = new AutoResetEvent(false);

        public Server(string ip, int port)
        {
            server = new TcpListener(IPAddress.Parse(ip), port);
        }

        public delegate void ConnecteEventHandler(object sender, TcpClient client);
        public event ConnecteEventHandler OnConnect;       

        public override void Run()
        {
            DebugLogger.AddLog("Server start accepting connection.");
            server.Start();
            while (IsRunning)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    if (OnConnect == null)
                    {
                        client.Close();
                    }
                    OnConnect(this, client);
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.Interrupted)
                    {
                        DebugLogger.AddLog("Server stoped.");
                        return;
                    }
                    DebugLogger.AddLog("SocketException:" + se.Message);
                }
                catch (Exception ex)
                {
                    DebugLogger.AddLog("Exception:" + ex);
                }
            }
        }

        public override void Stop()
        {
            server.Stop();
            base.Stop();
        }
    }
}
