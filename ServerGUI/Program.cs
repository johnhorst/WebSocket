using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WebSocket.Logger;
using WebSocket.Network;

namespace ServerGUI
{
    static class Program
    {    

      

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>    
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                StartGUI();
            }
            ParseArgs(args);
        }

        private static void ConsoleShow(bool show)
        {
            var handle = GetConsoleWindow();
            if (show)
            {
                if (handle == IntPtr.Zero)
                {
                    AllocConsole();
                    return;
                }
                ShowWindow(handle, SW_SHOW);
            }

            ShowWindow(handle, SW_HIDE);
        }

        static void ParseArgs(string[] args)
        {
            string ip = "0.0.0.0";
            int port = 2024;          
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-g":
                        StartGUI();                        
                        return;
                    case "-ip":
                        if(i == args.Length - 1)
                        {
                            Console.WriteLine("Invalid arguments");
                            return;
                        }
                        ip = args[++i];
                        break;
                    case "-port":
                        if (i == args.Length - 1)
                        {
                            Console.WriteLine("Invalid arguments");
                            return;
                        }
                        port = int.Parse(args[++i]);
                        break;
                    default:
                        break;
                }
            }
            StartConsole(ip, port);
        }
      
        static void StartGUI()
        {
            ConsoleShow(false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void StartConsole(string ip,int port)
        {           
            ServerManager sm = new ServerManager(ip,port);
            DebugLogger.OnMessage += DebugLogger_OnMessage;
            sm.ClientManager.OnUserCountChange += ClientManager_OnUserCountChange;
            sm.Start();
            while (!Console.ReadLine().Equals("quit")) ;
            sm.Stop(); 
        }

        private static void DebugLogger_OnMessage(object sender, string msg)
        {
            Console.WriteLine(msg);
        }

        private static void ClientManager_OnUserCountChange(object sender, int userOnline)
        {
            Console.WriteLine("UserOnline:" +userOnline);
        }
    }
}
