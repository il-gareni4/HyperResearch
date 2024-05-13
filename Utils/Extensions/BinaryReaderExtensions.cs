using System.Collections.Generic;
using System.IO;

namespace HyperResearch.Utils.Extensions;

public static class BinaryReaderExtensions
{
    public static int[] ReadInts32(this BinaryReader reader)
    {
        int count = reader.ReadInt32();
        var result = new int[count];
        for (var i = 0; i < count; i++)
            result[i] = reader.ReadInt32();
        return result;
    }

    public static Dictionary<int, int> ReadDictInt32Int32(this BinaryReader reader)
    {
        int count = reader.ReadInt32();
        Dictionary<int, int> result = new(count);
        for (var i = 0; i < count; i++)
            result[reader.ReadInt32()] = reader.ReadInt32();
        return result;
    }
}