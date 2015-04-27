using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocket.Logger;
using WebSocket.MyThread;
using WebSocket.Network.Packets;
using WebSocket.Utility;

namespace WebSocket.Network
{
    public class ClientManager : ThreadImpl
    {
        private List<IClient> addQueue;
        private List<IClient> removeQueue;
        public ClientManager()
        {
            Clients = new List<IClient>();
            addQueue = new List<IClient>();
            removeQueue = new List<IClient>();
        }

        public override void Stop()
        {
            base.Stop();

            foreach (IClient client in Clients)
            {
                client.CloseConnection -= null;
                client.TimeOutConnection -= null;
                client.Close();
            }
            Clients.Clear();
            if (OnUserCountChange != null)
                OnUserCountChange(this, 0);
        }

        public override void Run()
        {
            while (IsRunning)
            {
                try
                {
                    if (addQueue.Count > 0)
                    {
                        lock (addQueue)
                        {
                            foreach (IClient c in addQueue)
                                Clients.Add(c);
                            addQueue.Clear();
                            if (OnUserCountChange != null)
                                OnUserCountChange(this, Clients.Count);
                        }
                    }

                    if (removeQueue.Count > 0)
                    {
                        lock (removeQueue)
                        {
                            foreach (IClient c in removeQueue)
                                Clients.Remove(c);
                            removeQueue.Clear();
                            if (OnUserCountChange != null)
                                OnUserCountChange(this, Clients.Count);
                            if (Clients.Count == 0)
                                Stop();
                        }
                    }

                    foreach (IClient client in Clients)
                        if (client.IsConnected && client.DataAvailable)
                        {
                            while (client.DataAvailable)
                            {
                                byte[] buffer;
                                client.Read(out buffer);
                                if (buffer == null || buffer.Length == 0)
                                    continue;

                                Packet packet = Utill.FromJSON<Packet>(Encoding.UTF8.GetString(buffer));

                                HandleRequest(client, packet);
                            }
                        }

                    Thread.Sleep(2);
                }
                catch (Exception ex)
                {
                    DebugLogger.AddLog("Exception:" + ex.Message);
                }
            }
        }

        public delegate void UserCountEventHandler(object sender, int userOnline);

        public event UserCountEventHandler OnUserCountChange;


        public List<IClient> Clients { get; private set; }

        public void AddClient(IClient client)
        {
            if (!Clients.Contains(client))
            {
                lock (addQueue)
                {
                    addQueue.Add(client);
                }
                client.CloseConnection += Client_CloseConnection;
                client.TimeOutConnection += Client_TimeOutConnection;
                if (IsRunning == false)
                    Start();
            }
        }

        public void RemoveClient(IClient client)
        {
            if (Clients.Contains(client) && !removeQueue.Contains(client))
            {
                lock (removeQueue)
                {
                    removeQueue.Add(client);
                }
            }
        }

        private void HandleRequest(IClient client, Packet packet)
        {
            Packet aux;
            IDictionary<string, object> dict = (IDictionary<string, object>)packet.Message;
            switch (packet.Type.ToLower())
            {
                case "draw":
                    DrawPacket drawPacket = new DrawPacket();
                    drawPacket.X = (int)dict["X"];
                    drawPacket.Y = (int)dict["Y"];
                    drawPacket.Type = (string)dict["Type"];
                    switch (drawPacket.Type.ToLower())
                    {
                        case "start":
                            drawPacket.Color = (string)dict["Color"];
                            drawPacket.LineWidth = (int)dict["LineWidth"];
                            break;
                    }
                    aux = new Packet("draw", drawPacket);
                    BroadCast(client, aux);
                    break;
                case "chat":
                    ChatPacket chatPacket = new ChatPacket();
                    chatPacket.User = (string)dict["User"];
                    chatPacket.Message = (string)dict["Message"];
                    aux = new Packet("chat", chatPacket);
                    BroadCast(client, aux);
                    break;
                default:
                    break;
            }
        }

        private void BroadCast(IClient client, Packet packet)
        {
            foreach (IClient c in Clients)
                if (c != client && c.IsConnected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(Utill.ToJSON(packet));
                    if (buffer != null || buffer.Length > 0)
                        c.Write(buffer);
                }
        }

        private void Client_TimeOutConnection(object sender, ClientEvent e)
        {
            RemoveClient((IClient)sender);
        }

        private void Client_CloseConnection(object sender, ClientEvent e)
        {
            RemoveClient((IClient)sender);
        }
    }
}