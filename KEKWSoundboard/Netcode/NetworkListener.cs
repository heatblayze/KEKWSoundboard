using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KEKWSoundboard.Netcode
{
    internal class NetworkListener : IDisposable
    {
        TcpListener _server = null;
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public NetworkListener(int port = 15369)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            _server = new TcpListener(localAddr, port);
            _server.Start();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _server.BeginAcceptTcpClient(AcceptClient, null);
            }
        }

        void AcceptClient(IAsyncResult result)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var client = _server.EndAcceptTcpClient(result);            

            byte[] bytes = new byte[512];
            string data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.UTF8.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);

                byte[] msg = Encoding.UTF8.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }

            client.Close();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _server.Stop();
        }
    }
}
