using System.IO;

namespace Atom.Protocol.Messages
{
    public class LoginRequestMessage : INetworkMessage
    {
        // Fields
        public const short ProtocolId = 3;


        // Constructor
        public LoginRequestMessage()
        {
        }

        public LoginRequestMessage(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }


        // Properties
        public short MessageId => ProtocolId;


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Username);
            writer.Write(Password);
        }

        public void Deserialize(BinaryReader reader)
        {
            Username = reader.ReadString();
            Password = reader.ReadString();
        }
    }
}