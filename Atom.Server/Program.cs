using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atom.Core.Network;
using Atom.Protocol;
using Atom.Server.Clients;

namespace Atom.Server
{
    internal class Program
    {
        public static List<Client> Clients { get; private set; }

        private static void Main(string[] args)
        {
            Clients = new List<Client>();

            var server = new ServerWrapper(IPAddress.Any, 1458);
            server.ClientConnected += Server_ClientConnected;
            server.ErrorOccured += Server_ErrorOccured;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Start();

            Console.WriteLine("Server started.");
            Console.ReadKey();
        }

        private static void Server_ClientConnected(ClientHandler handler)
        {
            Clients.Add(new Client(handler));
            Console.WriteLine("Client connected.");
        }

        private static void Server_ErrorOccured(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        private static void Server_ClientDisconnected(ClientHandler handler)
        {
            var client = Clients.FirstOrDefault(c => c.Network == handler.Client);

            if (client == null) return;

            Clients.Remove(client);
            Console.WriteLine("Client {0} disconnected, {1} left.", client.Informations.ToString(), Clients.Count);

            // Dispose the client
            client.Dispose();
        }

        public static void BroadcastMessage(INetworkMessage message, bool onlyLoggedIn)
        {
            foreach (var c in Clients)
            {
                if (onlyLoggedIn && !c.LoggedIn)
                    continue;

                c.SendMessage(message);
                Console.WriteLine("Sent {0} to {1}.", message.GetType().Name, c.ToString());
            }
        }
    }
}