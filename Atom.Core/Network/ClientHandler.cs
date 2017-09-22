using System;
using System.Net.Sockets;
using System.Threading;
using Atom.Protocol;
using Atom.Protocol.Messages;

namespace Atom.Core.Network
{
    public class ClientHandler : IDisposable
    {
        private Timer _pingTimeoutTimer;

        // Fields
        private Timer _pingTimer;


        // Constructor
        public ClientHandler(Socket socket)
        {
            Client = new ClientWrapper(socket);
            _pingTimer = new Timer(Ping_Callback, null, 30000, 30000);
            _pingTimeoutTimer = new Timer(PingTimeout_Callback, null, Timeout.Infinite, Timeout.Infinite);

            Client.DataReceived += Client_DataReceived;
            Client.Disconnected += Client_Disconnected;
        }


        // Properties
        public ClientWrapper Client { get; private set; }

        public void Dispose()
        {
            Client.DataReceived -= Client_DataReceived;
            Client.Disconnected -= Client_Disconnected;

            _pingTimer.Dispose();
            _pingTimeoutTimer.Dispose();
            Client.Dispose();

            _pingTimer = null;
            _pingTimeoutTimer = null;
            Client = null;
        }


        private void Client_DataReceived(ClientWrapper client, byte[] data)
        {
            var message = MessagesBuilder.BuildMessage(data);
            if (message != null && message.MessageId == PongMessage.ProtocolId)
                _pingTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Client_Disconnected(ClientWrapper client)
        {
            _pingTimer.Dispose();
            _pingTimeoutTimer.Dispose();
        }

        private void Ping_Callback(object state)
        {
            if (!Client.Running)
                return;

            Client.SendMessage(new PingMessage(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
            _pingTimeoutTimer.Change(5000, Timeout.Infinite);
        }

        private void PingTimeout_Callback(object state)
        {
            if (!Client.Running)
                return;

            Console.WriteLine("Client ping timeout.");
            Client.Close();
        }
    }
}