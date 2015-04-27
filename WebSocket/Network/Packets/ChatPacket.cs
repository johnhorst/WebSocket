namespace WebSocket.Network.Packets
{
    public class ChatPacket
    {
        public ChatPacket()
        {

        }
        public ChatPacket(string user, string message)
        {
            User = user;
            Message = message;
        }
        public string User { get; set; }
        public string Message { get; set; }
    }
}
