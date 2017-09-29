using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Atom.Core.Network;
using Atom.Core.Utility.Extensions;
using Atom.Core.Utility.Security;
using Atom.Protocol;
using Atom.Protocol.Messages;
using Atom.Server.Handlers;

namespace Atom.Server.Clients
{
    public class Client : IDisposable
    {
        // Fields

        private Timer _pingTimeoutTimer;
        private Timer _pingTimer;

        // Constructor
        public Client(ClientWrapper clientWrapper)
        {
            Network = clientWrapper;
            Informations = new ClientInformations(this);

            _pingTimer = new Timer(Ping_Callback, null, 30000, 30000);
            _pingTimeoutTimer = new Timer(PingTimeout_Callback, null, Timeout.Infinite, Timeout.Infinite);

            Network.DataReceived += Network_DataReceived;
            Network.ErrorOccured += Network_ErrorOccured;
            Network.Disconnected += Network_Disconnected;
        }


        // Properties
        public ClientWrapper Network { get; private set; }

        public bool Running => Network.Running;
        public ClientInformations Informations { get; private set; }
        public bool LoggedIn { get; set; }

        public void SendMessage(INetworkMessage message, bool withoutMsg = false)
        {
            var bytes = new List<byte>();

            using (var writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write(message.MessageId);
                message.Serialize(writer);

                bytes.AddRange(AesHelpers.Encrypt(writer.GetAllBytes()));

                // Insert the message length in the beginning
                var length = (short) bytes.Count;
                bytes.Insert(0, (byte) (length >> 8));
                bytes.Insert(0, (byte) (length & 255));
            }

            Network.Send(bytes.ToArray());

            if (withoutMsg)
                return;

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
            _pingTimer.Dispose();
            _pingTimeoutTimer.Dispose();
            LoggedIn = false;
            Informations = null;
            Network = null;
            _pingTimer = null;
            _pingTimeoutTimer = null;
        }

        #endregion

        #region Ping/Pong

        private void Ping_Callback(object state)
        {
            SendMessage(new PingMessage(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), true);
            _pingTimeoutTimer.Change(5000, Timeout.Infinite);
        }

        private void PingTimeout_Callback(object state)
        {
            Console.WriteLine("Client {0} timed out.", Informations);
            Network.Close();
        }

        public void StopPingTimeoutTimer()
        {
            _pingTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion
    }
}