using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Server
{
    class Client
    {
        private TcpClient _tcpClient;
        private Server _server;
        private NetworkStream _stream;

        protected internal string ID { get; }
        protected internal string Name { get; private set; }

        public Client(TcpClient tcpClient, Server server)
        {
            ID = Guid.NewGuid().ToString();
            _tcpClient = tcpClient;
            _server = server;
            _server.AddConnection(this);
        }

        protected internal void Process()
        {
            try
            {
                _stream = _tcpClient.GetStream();

                //get name
                Name = GetMessage();

                //hello message
                string message = $"{Name} подключился";
                _server.Broadcast(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        _server.Broadcast($"[{Name}]: {message}");
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                _server.RemoveConnection(this);
                Close();
            }
        }

        private string GetMessage()
        {
            StringBuilder sb = new StringBuilder();
            byte[] data = new byte[256];
            int bytes = 0;

            do
            {
                bytes = _stream.Read(data, 0, data.Length);
                sb.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (_stream.DataAvailable);

            return sb.ToString();
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }

        protected internal void Close()
        {
            _stream?.Close();
            _tcpClient?.Close();
        }
    }
}
