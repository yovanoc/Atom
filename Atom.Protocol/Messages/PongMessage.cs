using System.IO;

namespace Atom.Protocol.Messages
{
    public class PongMessage : INetworkMessage
    {
        // Fields
        public const short ProtocolId = 2;

        // Constructor
        public PongMessage()
        {
        }

        public PongMessage(long delay)
        {
            Delay = delay;
        }

        public long Delay { get; private set; }


        // Properties
        public short MessageId => ProtocolId;


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Delay);
        }

        public void Deserialize(BinaryReader reader)
        {
            Delay = reader.ReadInt64();
        }
    }
}