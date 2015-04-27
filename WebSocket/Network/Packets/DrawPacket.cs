namespace WebSocket.Network.Packets
{
    public class DrawPacket
    {

        public DrawPacket()
        {

        }
        public DrawPacket(int x, int y, string type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int LineWidth { get; set; }
    }
}
