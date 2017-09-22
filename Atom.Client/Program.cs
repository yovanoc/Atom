using System;
using Atom.Client;
using Atom.Core.Network;
using Atom.Protocol.Messages;

namespace Network
{
    internal class Program
    {
        private static readonly ServerManager ServerManager = new ServerManager();

        private static void Main(string[] args)
        {
            ServerManager.MessagesManager.RegisterMessage<LoginAcceptedMessage>(HandleLoginAcceptedMessage);

            ServerManager.Network.Connected += Network_Connected;

            ServerManager.Start();

            Console.ReadKey();
        }

        private static void HandleLoginAcceptedMessage(ServerManager sm, LoginAcceptedMessage message)
        {
            Console.WriteLine("LOGIN ACCEPTED");
            sm.LoggedIn = true;
        }

        private static void Network_Connected(ClientWrapper client)
        {
            ServerManager.Network.SendMessage(new LoginRequestMessage("jesaispas", "password"));
        }
    }
}