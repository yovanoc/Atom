using System.IO;

namespace Atom.Core.Utility.Extensions
{
    public static class OtherExtensions
    {
        public static byte[] GetAllBytes(this BinaryWriter writer)
        {
            var pos = writer.BaseStream.Position;

            var data = new byte[writer.BaseStream.Length];
            writer.BaseStream.Position = 0;
            writer.BaseStream.Read(data, 0, (int) writer.BaseStream.Length);

            writer.BaseStream.Position = pos;

            return data;
        }
    }
}