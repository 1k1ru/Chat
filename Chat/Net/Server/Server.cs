using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chat.UI;

namespace Chat.Net.Server
{
    class Server
    {
        private List<Client> _clients = new List<Client>();
        private TcpListener _listener;
        private Thread _listenThread;
        private IPAddress _ip = IPAddress.Any;
        private int _port = 8888;
        private bool _stopped = false;

        private ChatForm _chat;

        public Server(ChatForm chat)
        {
            _chat = chat;
        }

        public Server(ChatForm chat, int port) : this(chat)
        {
            _port = port;
        }

        public Server(ChatForm chat, IPAddress ip, int port) : this(chat, port)
        {
            _ip = ip;
        }

        public void Start()
        {
            _listenThread = new Thread(Listen);
            _listenThread.Start();
            _stopped = false;

            Log("Server started");
        }

        protected internal void AddConnection(Client client)
        {
            _clients.Add(client);

            Log($"Client [ID:{client.ID}] connected");
        }

        protected internal void RemoveConnection(Client client)
        {
            _clients?.Remove(client);

            Log($"Client [ID:{client.ID}] disconnected");
            Broadcast($"{client.Name} отключился");
        }

        private void Listen()
        {
            try
            {
                _listener = new TcpListener(_ip, _port);
                _listener.Start();

                Log($"Listening port {_port}");

                while (true)
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    Client client = new Client(tcpClient, this);
                    Thread clientThread = new Thread(client.Process);
                    clientThread.Start();
                }
            }
            catch (Exception)
            {
                if (!_stopped)
                    Stop();
            }
        }

        public void Broadcast(string message)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(message);
            }

            PrintMessage(message);
        }

        public void Stop()
        {
            if (_stopped)
                return;

                _stopped = true;
            _listener.Stop();

            foreach (var client in _clients)
            {
                client.Close();
            }
            _clients.Clear();

            Log("Server stopped");
        }

        private void PrintMessage(string message)
        {
            _chat.BeginInvoke(new ChatForm.MessageHandler(ChatLog), message);
        }

        private void Log(string message)
        {
            _chat.BeginInvoke(new ChatForm.MessageHandler(ChatLog), $"[SERVER]: {message}");
        }

        private void ChatLog(string message)
        {
            _chat.Log(message);
        }
    }
}
