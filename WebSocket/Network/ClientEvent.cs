using System;

namespace WebSocket.Network
{

    public enum EventType
    {
        Disconnected,
        TimeOut,
        Connected
    }
    public class ClientEvent : EventArgs
    {
        public ClientEvent(EventType type)
        {
            Type = type;
        }

        public EventType Type { get; private set; }
    }
}
