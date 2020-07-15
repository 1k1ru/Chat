using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chat.Net.Client;
using Chat.Net.Server;

namespace Chat.UI
{
    public partial class ChatForm : Form
    {
        public delegate void MessageHandler(string message);
        public delegate void ConnectionHandler(string name, string hostname, int port);
        public delegate void HostingHandler(IPAddress ip, int port);

        private Server _server;
        private Net.Client.Client _client;

        public ChatForm()
        {
            InitializeComponent();
        }

        protected internal void Connect(string name, string hostname, int port)
        {
            _client = new Net.Client.Client(this, name);
            if (_client.Connect(hostname, port))
            {
                connectToolStripMenuItem.Enabled = false;
                disconnectToolStripMenuItem.Enabled = true;
                startServerToolStripMenuItem.Enabled = false;
            }
        }

        protected internal void Host(IPAddress ip, int port)
        {
            _server = new Server(this, ip, port);
            _server.Start();

            startServerToolStripMenuItem.Enabled = false;
            stopServerToolStripMenuItem.Enabled = true;
            connectToolStripMenuItem.Enabled = false;
        }

        protected internal void Log(string message)
        {
            chat.AppendText($"{message}\n");
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            _server?.Broadcast($"[SERVER]: {toSend.Text}");
            _client?.SendMessage(toSend.Text);
            toSend.Clear();
        }

        private void toSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _server?.Broadcast($"[SERVER]: {toSend.Text}");
                _client?.SendMessage(toSend.Text);
                toSend.Clear();
            }
        }

        private void toSend_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toSend.Clear();
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionForm connectionForm = new ConnectionForm(this);
            connectionForm.ShowDialog();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _client.Disconnect();
            _client = null;

            connectToolStripMenuItem.Enabled = true;
            disconnectToolStripMenuItem.Enabled = false;
            startServerToolStripMenuItem.Enabled = true;
        }

        private void startServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HostingForm hostingForm = new HostingForm(this);
            hostingForm.ShowDialog();
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.Stop();

            startServerToolStripMenuItem.Enabled = true;
            stopServerToolStripMenuItem.Enabled = false;
            connectToolStripMenuItem.Enabled = true;
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server?.Stop();
            _client?.Disconnect();
        }
    }
}
