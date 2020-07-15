using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat.UI
{
    public partial class ConnectionForm : Form
    {
        private ChatForm _chat;

        public ConnectionForm(ChatForm chat)
        {
            InitializeComponent();

            _chat = chat;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                string name = tb_name.Text;
                string hostname = tb_ip.Text;
                int port = int.Parse(tb_port.Text);

                _chat.BeginInvoke(new ChatForm.ConnectionHandler(Connect), name, hostname, port);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK);
            }
        }

        private void Connect(string name, string hostname, int port)
        {
            _chat.Connect(name, hostname, port);
            this.Close();
        }
    }
}
