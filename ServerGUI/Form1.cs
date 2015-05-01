using System;
using System.Windows.Forms;
using WebSocket.Logger;
using WebSocket.Network;

namespace ServerGUI
{
    public partial class Form1 : Form
    {
        ServerManager sm;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            DebugLogger.OnMessage += DebugLogger_OnMessage;
        }

        private void UpdateRichTextBox(string txt)
        {
            MethodInvoker invoker = new MethodInvoker(() =>
            {
                if (richTextBox1.Text.Length == 1000000)
                    richTextBox1.Text = "";
                richTextBox1.Text += txt + Environment.NewLine;
                richTextBox1.Select(richTextBox1.Text.Length - 1, 0);
                richTextBox1.ScrollToCaret();
            });
            try
            {
                BeginInvoke(invoker);
            }
            catch { }
        }

        private void DebugLogger_OnMessage(object sender, string msg)
        {
            UpdateRichTextBox(msg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            string[] addr = textBox1.Text.Split(':');
            sm = new ServerManager(addr[0], int.Parse(addr[1]));
            sm.ClientManager.OnUserCountChange += ClientManager_OnUserCountChange;

            sm.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            sm.Stop();
        }

        private void UpdateUserLabel(int userOnline)
        {
            MethodInvoker invoker = new MethodInvoker(() =>
            {
                label1.Text = "User Online:" + userOnline;
            });
            try
            {
                BeginInvoke(invoker);
            }
            catch { }
        }

        private void ClientManager_OnUserCountChange(object sender, int userOnline)
        {
            UpdateUserLabel(userOnline);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sm.Stop();
        }
    }
}
