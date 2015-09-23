using UnityEngine;
using System.Collections;
using FullSerializer;

public class DataObject<T> where T : class
{
    private static readonly fsSerializer serializer = new fsSerializer();

    public override string ToString()
    {
        fsData data;
        serializer.TrySerialize(this as T, out data);

        return fsJsonPrinter.PrettyJson(data);
    }

    public bool Deserialize(string serializedData)
    {
        fsResult result = DeserializeObject(serializedData, this as T);
        return result.Succeeded;
    }

    private static fsResult DeserializeObject(string serializedData, T target)
    {
        fsData data = fsJsonParser.Parse(serializedData);
        return serializer.TryDeserialize<T>(data, ref target);
    }
}
