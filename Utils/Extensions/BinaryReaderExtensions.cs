using System.IO;

namespace HyperResearch.Utils.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static int[] ReadInts32(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
                result[i] = reader.ReadInt32();
            return result;
        }
    }
}
