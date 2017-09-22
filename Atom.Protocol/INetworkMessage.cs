using System.IO;

namespace Atom.Protocol
{
    public interface INetworkMessage
    {
        short MessageId { get; }

        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}