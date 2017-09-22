using System.IO;

namespace Atom.Protocol.Messages
{
    public class PingMessage : INetworkMessage
    {
        // Fields
        public const short ProtocolId = 1;

        // Constructor
        public PingMessage()
        {
        }

        public PingMessage(long time)
        {
            Time = time;
        }

        public long Time { get; private set; }


        // Properties
        public short MessageId => ProtocolId;


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Time);
        }

        public void Deserialize(BinaryReader reader)
        {
            Time = reader.ReadInt64();
        }
    }
}