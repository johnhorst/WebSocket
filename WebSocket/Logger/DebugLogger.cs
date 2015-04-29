using System;
using System.IO;
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
            sb.Append(msg + Environment.NewLine);
            if (sb.Length == int.MaxValue)
            {
                File.AppendAllText("logger.txt", sb.ToString());
                sb.Clear();
            }
        }

        public static string GetLog()
        {
            return sb.ToString();
        }
    }
}
