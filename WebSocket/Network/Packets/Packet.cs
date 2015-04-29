using System;
using System.Text;
using WebSocket.Json;

namespace WebSocket.Network.Packets
{
    public class Packet : IJson
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

        public string Stringify()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\"Type\":\"" + Type +"\",");
            sb.Append("\"Message\":");
            if (Message is IJson)
            {
                sb.Append(((IJson)Message).Stringify()+ "}");
                return sb.ToString();
            }
            sb.Append(JsonConvertor.ToJSON(Message) + "}");
            return sb.ToString();
        }
    }
}
