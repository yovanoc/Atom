using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Atom.Core.Network
{
    public class ServerWrapper
    {
        // Fields
        private readonly TcpListener _tcpListener;


        // Constructor
        public ServerWrapper(IPAddress ipAddress, int port)
        {
            _tcpListener = new TcpListener(ipAddress, port);
            Clients = new List<ClientHandler>();
        }


        // Properties
        public bool Listening { get; private set; }

        public List<ClientHandler> Clients { get; }


        // Events
        public event Action<ClientHandler> ClientConnected;

        public event Action<ClientHandler> ClientDisconnected;
        public event Action<Exception> ErrorOccured;


        public void Start()
        {
            if (Listening)
                return;

            try
            {
                _tcpListener.Start();
                Listening = true;

                _tcpListener.BeginAcceptSocket(TcpListener_AcceptCallback, _tcpListener);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        public void Stop()
        {
            if (!Listening)
                return;

            try
            {
                _tcpListener.Stop();
                Listening = false;
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        #region Callbacks

        private void TcpListener_AcceptCallback(IAsyncResult ar)
        {
            if (!Listening)
                return;

            try
            {
                var newClient = (ar.AsyncState as TcpListener).EndAcceptSocket(ar);
                var clientHandler = new ClientHandler(newClient);

                clientHandler.Client.Disconnected += Client_Disconnected;

                Clients.Add(clientHandler);
                clientHandler.Client.Start();
                ClientConnected?.Invoke(clientHandler);

                // Re-call BeginAcceptSocket
                _tcpListener.BeginAcceptSocket(TcpListener_AcceptCallback, _tcpListener);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        private void Client_Disconnected(ClientWrapper client)
        {
            try
            {
                var clientHandler = Clients.FirstOrDefault(c => c.Client == client);

                if (clientHandler != null)
                {
                    Clients.Remove(clientHandler);
                    ClientDisconnected?.Invoke(clientHandler);

                    clientHandler.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        #endregion
    }
}