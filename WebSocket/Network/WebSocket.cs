using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebSocket.Logger;

namespace WebSocket.Network
{
    public class WebSocket : Client
    {
        private enum OpCodeType
        {
            CONTINUATION_FRAME = 0x0,
            TEXT_FRAME = 0x1,
            BINARY_FRAME = 0x2,
            CONNECTION_CLOSE = 0x8,
            PING = 0x9,
            PONG = 0xA
        }
        public WebSocket() : base()
        {

        }

        public WebSocket(TcpClient client) : base(client)
        {

        }

        public override TcpClient Socket
        {
            get
            {
                return base.Socket;
            }

            set
            {
                base.Socket = value;
                if (!UpgrateConnection())
                    throw new WebSocketException();
            }
        }


        public override void Close()
        {
            if (IsConnected)
                Write(null, OpCodeType.CONNECTION_CLOSE);
            base.Close();
        }

        public override int Read(out byte[] buffer, int size)
        {
            byte[] header;
            base.Read(out header, 6);

            OpCodeType opCode = (OpCodeType)(header[0] & 0x0F);          

            int i = 2;

            int dataSize = (header[1] & (~0x80));            
            if (dataSize == 126)
            {
                dataSize += BitConverter.ToInt16(header, i);              
                i = 4;
                Array.Resize(ref header, 10);
                Read(ref header, 6, 2);
            }
            else if (dataSize == 127)
            {                
                i = 10;              
                Array.Resize(ref header, 14);
                Read(ref header, 6, 14);
                dataSize += (int)BitConverter.ToInt64(header, 2);
            }           

            switch (opCode)
            {
                case OpCodeType.CONTINUATION_FRAME:
                    break;
                case OpCodeType.TEXT_FRAME:
                    break;
                case OpCodeType.BINARY_FRAME:
                    break;
                case OpCodeType.CONNECTION_CLOSE:
                    Close();
                    buffer = null;
                    return 0;                    
                case OpCodeType.PING:
                    Write(null, OpCodeType.PONG);
                    break;
                case OpCodeType.PONG:
                    break;
                default:
                    break;
            }

            byte[] mask = new byte[4];
            Array.Copy(header, i, mask, 0, 4);
           
            base.Read(out buffer, dataSize);

            if(buffer!=null)
            {
                for (i = 0; i < buffer.Length; i++)
                    buffer[i] = (byte)(buffer[i] ^ mask[i % 4]);
                //DebugLogger.AddLog("Size:" +dataSize + "\nRecive:" + Encoding.UTF8.GetString(buffer));
                return buffer.Length;
            }
            return 0;
        }

        public override void Write(byte[] buffer)
        {
            Write(buffer, OpCodeType.TEXT_FRAME);
        }

        private void Write(byte[] buffer, OpCodeType opCode)
        {
            byte[] header = new byte[10];
            int size = buffer == null ? 0 : buffer.Length;
            header[0] = (byte)((1 << 7) | (byte)opCode);
            int i = -1;

            if (i <= size)
            {
                header[1] = (byte)size;
                i = 2;
            }
            else if (size >= 126 && size <= 65535)
            {
                header[1] = 126;
                header[2] = (byte)((size >> 8) & 255);
                header[3] = (byte)((size) & 255);
                i = 4;
            }
            else
            {
                header[1] = 127;
                header[2] = (byte)((size >> 56) & 255);
                header[3] = (byte)((size >> 48) & 255);
                header[4] = (byte)((size >> 40) & 255);
                header[5] = (byte)((size >> 32) & 255);
                header[6] = (byte)((size >> 24) & 255);
                header[7] = (byte)((size >> 16) & 255);
                header[8] = (byte)((size >> 8) & 255);
                header[9] = (byte)((size) & 255);
                i = 10;
            }

            if (buffer != null)
            {
                Array.Resize(ref header, i + size);
                Array.Copy(buffer, 0, header, i, size);
            }
            base.Write(header);
        }

        private bool UpgrateConnection()
        {
            while (Size == 0) ;
            byte[] buffer;
            base.Read(out buffer, Size);
            string header = Encoding.ASCII.GetString(buffer);

            if (new Regex("^GET").IsMatch(header))
            {
                byte[] response = Encoding.ASCII.GetBytes("HTTP/1.1 101 Web Socket Protocol Handshake" + Environment.NewLine
                    + "Connection: Upgrade" + Environment.NewLine
                    + "Upgrade: websocket" + Environment.NewLine
                    + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                    SHA1.Create().ComputeHash(
                        Encoding.UTF8.GetBytes(
                            new Regex("Sec-WebSocket-Key: (.*)").Match(header).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                        )
                     )) + Environment.NewLine + Environment.NewLine);

                base.Write(response);
                return true;
            }

            return false;
        }
    }
}
