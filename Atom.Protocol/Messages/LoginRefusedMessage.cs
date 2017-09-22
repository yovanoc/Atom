using System.IO;
using Atom.Protocol.Enums;

namespace Atom.Protocol.Messages
{
    public class LoginRefusedMessage : INetworkMessage
    {
        // Fields
        public const short ProtocolId = 5;


        // Constructor
        public LoginRefusedMessage()
        {
        }

        public LoginRefusedMessage(LoginRefusedReason reason)
        {
            Reason = reason;
        }

        public LoginRefusedReason Reason { get; private set; }


        // Properties
        public short MessageId => ProtocolId;


        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte) Reason);
        }

        public void Deserialize(BinaryReader reader)
        {
            Reason = (LoginRefusedReason) reader.ReadByte();
        }
    }
}