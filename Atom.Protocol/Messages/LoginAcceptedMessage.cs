using System.IO;

namespace Atom.Protocol.Messages
{
    public class LoginAcceptedMessage : INetworkMessage
    {
        // Fields
        public const short ProtocolId = 4;


        // Constructor
        public LoginAcceptedMessage()
        {
        }

        public LoginAcceptedMessage(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }


        // Properties
        public short MessageId => ProtocolId;


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
        }

        public void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
    }
}