using System;
using System.Net.Sockets;
using System.Text;
using WebSocket.Logger;
using WebSocket.Utility;

namespace WebSocket.Network
{
    public class Client : IClient
    {
        private TcpClient socket;
        private NetworkStream ns;

        public event CloseEventHandler CloseConnection;
        public event TimeOutEventHandler TimeOutConnection;

        public Client()
        {

        }

        public Client(TcpClient client)
        {
            Socket = client;
        }

        public virtual TcpClient Socket
        {
            get { return socket; }
            set
            {
                socket = value;
                if (socket != null)
                {
                    ns = socket.GetStream();                    
                }
            }
        }


        public virtual bool DataAvailable
        {
            get
            {
                if (socket.Connected && socket.Available > 0)
                    return true;
                return false;
            }
        }

        public virtual int Size { get { return socket.Connected == true ? socket.Available : 0; } }

        public virtual bool IsConnected
        {
            get
            {
                return socket.Client.Connected;
            }
        }


        public virtual void Close()
        {
            if (CloseConnection != null)
                CloseConnection(this, new ClientEvent(EventType.Disconnected));
            socket.Close();
        }

        public virtual void Write(byte[] buffer)
        {
            if (buffer.Length == 0)
                return;
            try
            {
                ns.Write(buffer, 0, buffer.Length);
            }
            catch (SocketException se)
            {
                DebugLogger.AddLog("Socket Exception:" + se.Message);
                if (TimeOutConnection != null)
                    TimeOutConnection(this, new ClientEvent(EventType.TimeOut));
            }
            catch (Exception ex)
            {
                DebugLogger.AddLog("Exception:" + ex.Message);
            }
        }

        public void Write(string buffer)
        {
            if (buffer.Length == 0)
                return;

            byte[] byteBuffer = Encoding.ASCII.GetBytes(buffer);
            Write(byteBuffer);
        }


        public void Write<T>(T obj) where T : struct
        {
            byte[] buffer = Utill.ToBinary(obj);
            Write(buffer);
        }



        public virtual int Read(out byte[] buffer, int size)
        {
            try
            {
                buffer = new byte[size];
                int readed = 0;
                while (socket.Available != 0 && readed < size)
                {
                    readed = ns.Read(buffer, readed, size);
                }
                return readed;
            }
            catch (SocketException se)
            {
                DebugLogger.AddLog("Socket Exception:" + se.Message);
                if (TimeOutConnection != null)
                    TimeOutConnection(this, new ClientEvent(EventType.TimeOut));
            }
            catch (Exception ex)
            {
                DebugLogger.AddLog("Exception:" + ex.Message);
            }
            buffer = null;
            return 0;
        }

        public int Read(out string buffer, int size)
        {

            byte[] byteBuffer;
            int readed = Read(out byteBuffer, size);

            buffer = Encoding.UTF8.GetString(byteBuffer);

            return readed;
        }

        public int Read<T>(out T obj) where T : struct
        {
            int size = Utill.SizeOf<T>();

            byte[] buffer = null;

            Read(out buffer, size);
            obj = Utill.FromBinary<T>(buffer);
            return size;
        }

        public int Read(out byte[] buffer)
        {
            return Read(out buffer, Size);
        }

        public int Read(out string buffer)
        {
            return Read(out buffer, Size);
        }

        public int Read(ref byte[] buffer, int offset, int size)
        {
            byte[] buff = new byte[size];
            int readed = Read(out buff, size);
            Array.Copy(buff, 0, buffer, offset, readed);
            return readed;
        }
    }
}
