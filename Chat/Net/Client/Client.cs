using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chat.UI;

namespace Chat.Net.Client
{
    class Client
    {
        private TcpClient _client;
        private NetworkStream _networkStream;

        private ChatForm _chat;

        public string Name { get; }

        public Client(ChatForm chat, string name)
        {
            _chat = chat;
            Name = name;
        }

        public bool Connect(string hostname, int port)
        {
            try
            {
                //connecting
                _client = new TcpClient();
                _client.Connect(hostname, port);
                _networkStream = _client.GetStream();

                Log($"Connected to {hostname}:{port}");

                //send name for hello message
                SendMessage(Name);

                //listen thread
                Thread receiveThread = new Thread(ReceiveData);
                receiveThread.Start();

                return true;
            }
            catch (SocketException)
            {
                Log("Connection failed");
                return false;
            }
        }

        public void Disconnect()
        {
            _networkStream?.Close();
            _client?.Close();

            Log("Disconnected");
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            _networkStream.Write(data, 0, data.Length);
        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] data = new byte[256];
                    int bytes = 0;

                    do
                    {
                        bytes = _networkStream.Read(data, 0, data.Length);
                        if (bytes == 0)
                        {
                            throw new Exception();
                        }
                        sb.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (_networkStream.DataAvailable);

                    PrintMessage(sb.ToString());
                }
                catch (Exception)
                {
                    Log("Connection closed");

                    Disconnect();
                    break;
                }
            }
        }

        private void PrintMessage(string message)
        {
            _chat.BeginInvoke(new ChatForm.MessageHandler(ChatLog), message);
        }

        private void Log(string message)
        {
            _chat.BeginInvoke(new ChatForm.MessageHandler(ChatLog), $"[INFO]: {message}");
        }

        private void ChatLog(string message)
        {
            _chat.Log(message);
        }
    }
}
