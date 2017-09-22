using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Atom.Protocol;
using Atom.Server.Clients;

namespace Atom.Server.Handlers
{
    public static class HandlersManager
    {
        // Fields
        private static readonly Dictionary<string, List<MethodInfo>> Methods;


        static HandlersManager()
        {
            Methods = new Dictionary<string, List<MethodInfo>>();

            bool IsMethodValid(MethodInfo method)
            {
                return method.IsStatic && method.Name.StartsWith("Handle") && method.GetParameters().Length == 2 &&
                       method.ReturnType == typeof(Task);
            }

            foreach (var type in typeof(HandlersManager).Assembly.GetTypes())
            foreach (var method in type.GetMethods())
            {
                if (!IsMethodValid(method))
                    continue;

                var msgName = method.Name.Substring(6);

                if (!Methods.ContainsKey(msgName))
                    Methods.Add(msgName, new List<MethodInfo>());

                Methods[msgName].Add(method);
            }
        }


        public static void HandleMessage(Client client, INetworkMessage message)
        {
            if (message == null)
                return;

            var msgName = message.GetType().Name;

            if (!Methods.ContainsKey(msgName))
                return;

            for (var i = 0; i < Methods[msgName].Count; i++)
                Methods[msgName][i].Invoke(null, new object[] {client, message});
        }
    }
}