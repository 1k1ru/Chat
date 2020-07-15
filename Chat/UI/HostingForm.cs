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

namespace Chat.UI
{
    public partial class HostingForm : Form
    {
        private ChatForm _chat;

        public HostingForm(ChatForm chat)
        {
            InitializeComponent();

            _chat = chat;
        }

        private void hostButton_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(tb_ip.Text);
                int port = int.Parse(tb_port.Text);

                _chat.BeginInvoke(new ChatForm.HostingHandler(Host), ip, port);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK);
            }
        }

        private void Host(IPAddress ip, int port)
        {
            _chat.Host(ip, port);
            this.Close();
        }
    }
}
