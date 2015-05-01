using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocket.Logger;
using WebSocket.MyThread;
using WebSocket.Network.Packets;
using WebSocket.Json;

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
            if (Clients.Count > 0)
            {
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
                                SleepMode();
                        }
                    }


                    foreach (IClient client in Clients)
                    {
                        if (client.IsConnected == false)
                        {
                            client.Close();
                        }
                        if (client.DataAvailable)
                        {
                            while (client.DataAvailable)
                            {
                                byte[] buffer;
                                client.Read(out buffer);
                                if (buffer == null || buffer.Length == 0)
                                    continue;
                                Packet packet = JsonConvertor.FromJSON<Packet>(Encoding.UTF8.GetString(buffer));
                                HandleRequest(client, packet);
                            }
                        }
                    }
                       
                    if (Clients.Count < 250)
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
                if (IsSleepMode == true)
                    WakeUp();
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
                    drawPacket.Type = (string)dict["Type"];
                    switch (drawPacket.Type.ToLower())
                    {
                        case "start":
                            drawPacket.Color = (string)dict["Color"];
                            drawPacket.LineWidth = (int)dict["LineWidth"];
                            drawPacket.X = (int)dict["X"];
                            drawPacket.Y = (int)dict["Y"];
                            break;
                        case "move":
                            drawPacket.X = (int)dict["X"];
                            drawPacket.Y = (int)dict["Y"];
                            break;
                        case "clearscreen":
                            break;
                        default:
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
            byte[] buffer;
            try
            {
                buffer = Encoding.UTF8.GetBytes(JsonConvertor.ToJSON(packet));
                if (buffer == null || buffer.Length == 0)
                    return;
            }
            catch (Exception ex)
            {
                DebugLogger.AddLog("Exception:" + ex);
                return;
            }
            foreach (IClient c in Clients)
                if (c != client && c.IsConnected)
                    c.Write(buffer);
        }

        private void Client_TimeOutConnection(object sender, ClientEvent e)
        {
            DebugLogger.AddLog("TimeOut:" + sender.ToString());
            RemoveClient((IClient)sender);
        }

        private void Client_CloseConnection(object sender, ClientEvent e)
        {
            DebugLogger.AddLog("CloseConnection:" + sender.ToString());
            RemoveClient((IClient)sender);
        }
    }
}