using System;
using System.IO;
using Atom.Core.Utility.Security;

namespace Atom.Core.Network
{
    public class MessageParser : IDisposable
    {
        private int _currentMessageLength;

        // Fields
        private BinaryReader _reader;


        // Constructor
        public MessageParser()
        {
            _reader = new BinaryReader(new MemoryStream());
        }


        // Properties
        private int DataLength => (int) (_reader.BaseStream.Length - _reader.BaseStream.Position);

        public void Dispose()
        {
            _reader.Dispose();
            _reader = null;
        }


        // Event
        public event Action<byte[]> MessageParsed;


        public void HandleData(byte[] data)
        {
            if (data?.Length == 0)
                return;

            AddData(data);
            ProcessData();
        }

        private void ProcessData()
        {
            while (true)
            {
                // If the parser is already handling a message
                if (_currentMessageLength != 0)
                {
                    // If the reader has all the bytes we need
                    if (DataLength >= _currentMessageLength)
                    {
                        var bytes = _reader.ReadBytes(_currentMessageLength);
                        _currentMessageLength = 0;

                        MessageParsed?.Invoke(AesHelpers.Decrypt(bytes));
                        continue;
                    }
                }
                // Otherwise check if we can read the message's length
                else if (DataLength >= 2)
                {
                    _currentMessageLength = _reader.ReadInt16();
                    continue;
                }
                // If we can't read anything anymore, might aswell clear the stream
                else if (DataLength == 0)
                {
                    _reader.Dispose();
                    _reader = new BinaryReader(new MemoryStream());
                }
                break;
            }
        }

        private void AddData(byte[] data)
        {
            var pos = _reader.BaseStream.Position;
            _reader.BaseStream.Position = _reader.BaseStream.Length;
            _reader.BaseStream.Write(data, 0, data.Length);
            _reader.BaseStream.Position = pos;
        }
    }
}