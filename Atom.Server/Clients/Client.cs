using System;
using Atom.Core.Network;
using Atom.Protocol;
using Atom.Server.Handlers;

namespace Atom.Server.Clients
{
    public class Client : IDisposable
    {
        // Fields
        private readonly ClientHandler _handler;


        // Constructor
        public Client(ClientHandler handler)
        {
            _handler = handler;
            Informations = new ClientInformations(this);

            Network.DataReceived += Network_DataReceived;
            Network.ErrorOccured += Network_ErrorOccured;
            Network.Disconnected += Network_Disconnected;
        }


        // Properties
        public ClientWrapper Network => _handler.Client;

        public bool Running => Network.Running;
        public ClientInformations Informations { get; private set; }
        public bool LoggedIn { get; set; }

        public void SendMessage(INetworkMessage message)
        {
            Network.SendMessage(message);
            Console.WriteLine("{0} sent to client {1}.", message.GetType().Name, Informations.ToString());
        }

        #region Client events

        private void Network_DataReceived(ClientWrapper client, byte[] data)
        {
            try
            {
                var message = MessagesBuilder.BuildMessage(data);

                // If the message failed to build, do nothing
                if (message == null)
                    return;

                Console.WriteLine("Received {0} from client {1}.", message.GetType().Name, Informations.ToString());
                HandlersManager.HandleMessage(this, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured with client {0}, message: {1}.", Informations.ToString(),
                    ex.ToString());
            }
        }

        private void Network_ErrorOccured(ClientWrapper client, Exception exception)
        {
            Console.WriteLine("Exception occured in client {0}, informations: {1}.", Informations.ToString(),
                exception.ToString());
        }

        private void Network_Disconnected(ClientWrapper client)
        {
            LoggedIn = false;
        }

        public void Dispose()
        {
            Informations.Dispose();
            Informations = null;
            LoggedIn = false;
        }

    #endregion
}
}