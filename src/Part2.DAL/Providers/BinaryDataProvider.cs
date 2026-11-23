using System.Runtime.Serialization.Formatters.Binary;

namespace Lab33.DAL.Providers;

public class BinaryDataProvider<T> : DataProvider<T> where T : class
{
    public override string Extension => ".bin";

    protected override void Serialize(Stream stream, List<T> items) =>
        new BinaryFormatter().Serialize(stream, items);

    protected override List<T> Deserialize(Stream stream) =>
        (List<T>)new BinaryFormatter().Deserialize(stream);
}
