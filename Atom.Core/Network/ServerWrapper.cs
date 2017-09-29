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
            Clients = new List<ClientWrapper>();
        }


        // Properties
        public bool Listening { get; private set; }

        public List<ClientWrapper> Clients { get; }


        // Events
        public event Action<ClientWrapper> ClientConnected;

        public event Action<ClientWrapper> ClientDisconnected;
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
                var ClientWrapper = new ClientWrapper(newClient);

                ClientWrapper.Disconnected += Client_Disconnected;

                Clients.Add(ClientWrapper);
                ClientWrapper.Start();
                ClientConnected?.Invoke(ClientWrapper);

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
                var ClientWrapper = Clients.FirstOrDefault(c => c == client);

                if (ClientWrapper != null)
                {
                    Clients.Remove(ClientWrapper);
                    ClientDisconnected?.Invoke(ClientWrapper);

                    ClientWrapper.Dispose();
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