using System.Threading;
using System.Threading.Tasks;
using Atom.Protocol.Messages;
using Atom.Server.Clients;

namespace Atom.Server.Handlers
{
    public static class LoginHandlers
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public static Task HandleLoginRequestMessage(Client client, LoginRequestMessage message)
        {
            return Task.Run(async () =>
            {
                await Semaphore.WaitAsync();

                client.LoggedIn = true;
                client.SendMessage(new LoginAcceptedMessage("Chris"));
                client.Informations.Id = 21;
                client.Informations.Name = "Chris";

                Semaphore.Release();
            });
        }
    }
}