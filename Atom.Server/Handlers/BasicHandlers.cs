using System;
using System.Threading;
using System.Threading.Tasks;
using Atom.Protocol.Messages;
using Atom.Server.Clients;

namespace Atom.Server.Handlers
{
    public static class BasicHandlers
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public static Task HandlePongMessage(Client client, PongMessage message)
        {
            return Task.Run(async () =>
            {
                await Semaphore.WaitAsync();

                Console.WriteLine("RECEIVED A PONG FROM {0} WITH A DELAY OF {1} MS", client.Informations.ToString(),
                    message.Delay);
                client.StopPingTimeoutTimer();

                Semaphore.Release();
            });
        }
    }
}