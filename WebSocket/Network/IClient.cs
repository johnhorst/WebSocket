using System.Net.Sockets;

namespace WebSocket.Network
{
    public delegate void CloseEventHandler(object sender, ClientEvent e);
    public delegate void TimeOutEventHandler(object sender, ClientEvent e);
   
    public interface IClient
    {
        event CloseEventHandler CloseConnection;
        event TimeOutEventHandler TimeOutConnection;
        bool DataAvailable { get; }

        TcpClient Socket { get; }
        
        bool IsConnected { get; }
        int Size { get; }
        void Close();

        void Write(string buffer);
        void Write(byte[] buffer);
        void Write<T>(T obj) where T :struct;
        int Read(out string buffer,int size);
        int Read(out byte[] buffer,int size);
        int Read(ref byte[] buffer, int offset, int size);

        int Read(out byte[] buffer);
        int Read(out string buffer);
        int Read<T>(out T obj) where T : struct;        
    }
}
