using System;
using System.Collections.Generic;
using System.IO;

namespace Atom.Protocol
{
    public static class MessagesBuilder
    {
        // Fields
        private static readonly Dictionary<short, Type> Messages;


        // Constructor
        static MessagesBuilder()
        {
            Messages = new Dictionary<short, Type>();
            var ismType = typeof(INetworkMessage);

            foreach (var type in ismType.Assembly.GetTypes())
            {
                if (ismType == type)
                    continue;

                if (!ismType.IsAssignableFrom(type)) continue;
                var protocolIdField = type.GetField("ProtocolId");
                var messageId = Convert.ToInt16(protocolIdField.GetValue(type));

                Messages.Add(messageId, type);
            }
        }


        public static INetworkMessage BuildMessage(byte[] data)
        {
            // If the data doesn't even have enough for the message id
            if (data == null || data.Length <= 2)
                return null;

            INetworkMessage message = null;

            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                var messageId = reader.ReadInt16();

                if (Messages.ContainsKey(messageId))
                {
                    message = Activator.CreateInstance(Messages[messageId]) as INetworkMessage;
                    message.Deserialize(reader);
                }
            }

            return message;
        }
    }
}