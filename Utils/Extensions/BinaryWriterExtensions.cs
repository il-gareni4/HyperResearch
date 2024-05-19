using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HyperResearch.Utils.Extensions;

public static class BinaryWriterExtensions
{
    public static void Write(this BinaryWriter writer, IEnumerable<int> values)
    {
        int[] valuesArray = values.ToArray();
        writer.Write(valuesArray.Length);
        foreach (int value in valuesArray)
            writer.Write(value);
    }

    public static void Write(this BinaryWriter writer, IDictionary<int, int> values)
    {
        writer.Write(values.Count);
        foreach ((int key, int value) in values)
        {
            writer.Write(key);
            writer.Write(value);
        }
    }
}