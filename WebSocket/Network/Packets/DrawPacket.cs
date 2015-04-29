using System;
using System.Text;
using WebSocket.Json;
using System.Web.Script.Serialization;

namespace WebSocket.Network.Packets
{
    public class DrawPacket : IJson
    {

        public DrawPacket()
        {

        }

        public DrawPacket(string type)
        {
            Type = type;
        }
        public DrawPacket(string type, int x, int y)
        {
            X = x;
            Y = y;
            Type = type;
        }

        public DrawPacket(string type, string color, int lineWidth, int x, int y)
        {
            X = x;
            Y = y;
            Color = color;
            LineWidth = lineWidth;
            Type = type;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int LineWidth { get; set; }

        public string Stringify()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            switch (Type)
            {
                case "start":
                    sb.Append(string.Format("{0},{1},{2},{3},{4}", "\"Type\":\"" + Type + "\"",
                        "\"Color\":\"" + Color + "\"",
                        "\"LineWidth\":" + LineWidth,
                        "\"X\":" + X,
                        "\"Y\":" + Y));
                    break;
                case "move":
                    sb.Append(string.Format("{0},{1},{2}",
                        "\"Type\":\"" + Type + "\"",
                        "\"X\":" + X,
                        "\"Y\":" + Y));
                    break;                
                case "clearscreen":
                    sb.Append(string.Format("{0}", "\"Type\":\"" + Type + "\""));
                    break;
                default:
                    sb.Clear();
                    sb.Append(new JavaScriptSerializer().Serialize(this));
                    return sb.ToString();
            }
            sb.Append("}");
            return sb.ToString();
        }

    }
}
