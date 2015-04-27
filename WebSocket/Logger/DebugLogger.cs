using System;
using System.Text;

namespace WebSocket.Logger
{
    public static class DebugLogger
    {
        private static StringBuilder sb = new StringBuilder();

        public delegate void OnMessageEventHandler(object sender, string msg);
        public static event OnMessageEventHandler OnMessage;

        public static void AddLog(string msg)
        {
            if (OnMessage != null)
                OnMessage(null, msg);
           // sb.Append(msg + Environment.NewLine);
        }

        public static string GetLog()
        {
            return sb.ToString();
        }
    }
}
