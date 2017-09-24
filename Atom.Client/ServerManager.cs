using System;
using System.Net.Sockets;
using Atom.Core.Network;
using Atom.Protocol.Messages;

namespace Atom.Client
{
    public class ServerManager
    {
        // Constructor
        public ServerManager()
        {
            MessagesManager = new MessagesManager<ServerManager>(this);

            State = ServerConnectionState.Disconnected;

            MessagesManager.RegisterMessage<PingMessage>(HandlePingMessage);

            Network = new ClientWrapper();
            Network.Connected += Client_Connected;
            Network.ErrorOccured += Client_ErrorOccured;
            Network.DataReceived += Client_DataReceived;
            Network.Disconnected += Client_Disconnected;
        }

        // Properties
        public ClientWrapper Network { get; }

        public bool LoggedIn { get; set; }
        public ServerConnectionState State { get; set; }

        public MessagesManager<ServerManager> MessagesManager { get; set; }

        private void HandlePingMessage(ServerManager sm, PingMessage message)
        {
            sm.Network.SendMessage(new PongMessage(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - message.Time));
        }

        public void Start()
        {
            if (Network.Running)
                return;

            Network.Connect("127.0.0.1", 1458);
        }

        #region Client Events

        private void Client_Disconnected(ClientWrapper obj)
        {
            Console.WriteLine("Disconnected.");
            State = ServerConnectionState.Disconnected;
        }

        private void Client_DataReceived(ClientWrapper client, byte[] data)
        {
            if (State != ServerConnectionState.Connected)
                return;

            MessagesManager.HandleMessage(data);
        }

        private void Client_ErrorOccured(ClientWrapper client, Exception exception)
        {
            Console.WriteLine(exception.Message);

            if (!LoggedIn && exception is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused)
                Environment.Exit(0);
        }

        private void Client_Connected(ClientWrapper obj)
        {
            Console.WriteLine("Connected.");
            State = ServerConnectionState.Connected;
        }

        #endregion
    }
}