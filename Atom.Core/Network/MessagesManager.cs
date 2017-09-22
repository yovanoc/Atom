using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Atom.Protocol;

namespace Atom.Core.Network
{
    public class MessagesManager<TC>
    {
        private readonly ConcurrentDictionary<Type, List<Action<object>>> _registeredMessages;
        private readonly TC _typeClass;

        public MessagesManager(TC typeClass)
        {
            _typeClass = typeClass;
            _registeredMessages = new ConcurrentDictionary<Type, List<Action<object>>>();
        }

        public void RegisterMessage<T>(Action<TC, T> handler) where T : INetworkMessage
        {
            var msgType = typeof(T);
            if (!_registeredMessages.ContainsKey(msgType))
                _registeredMessages.TryAdd(msgType, new List<Action<object>>());

            _registeredMessages[msgType].Add(m => handler(_typeClass, (T) m));
        }

        public void HandleMessage(byte[] data)
        {
            var message = MessagesBuilder.BuildMessage(data);

            if (message == null)
                return;

            var type = message.GetType();
            if (!_registeredMessages.ContainsKey(type)) return;

            foreach (var action in _registeredMessages[type])
                action.Invoke(message);
        }
    }
}