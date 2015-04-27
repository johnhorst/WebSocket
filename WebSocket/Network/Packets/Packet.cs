namespace WebSocket.Network.Packets
{
    public class Packet
    {
        public Packet()
        {

        }
        public Packet(string type,object message)
        {
            Type = type;
            Message = message;
        }
        public string Type { get; set; }
        public object Message { get; set; }
    }
}
