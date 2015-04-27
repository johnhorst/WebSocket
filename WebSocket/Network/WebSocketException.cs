using System;

namespace WebSocket.Network
{
    public class WebSocketException : Exception
    {
        public WebSocketException() : base("Failed to upgrate the socket.")
        {

        }
    }
}
