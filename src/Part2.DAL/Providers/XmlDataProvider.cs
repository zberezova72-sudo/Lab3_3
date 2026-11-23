using System.Xml.Serialization;

namespace Lab33.DAL.Providers;

public class XmlDataProvider<T> : DataProvider<T> where T : class
{
    private static readonly XmlSerializer Serializer = new(typeof(List<T>));

    public override string Extension => ".xml";

    protected override void Serialize(Stream stream, List<T> items) =>
        Serializer.Serialize(stream, items);

    protected override List<T> Deserialize(Stream stream) =>
        (List<T>)Serializer.Deserialize(stream)!;
}
