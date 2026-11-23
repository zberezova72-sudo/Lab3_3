using System.Xml.Serialization;

namespace Lab33.Part1.Serialization;

public class XmlDataSerializer : DataSerializer
{
    public override string Name => "XML";
    public override string Extension => ".xml";

    protected override void Serialize<T>(Stream stream, T data) =>
        new XmlSerializer(typeof(T)).Serialize(stream, data);

    protected override T Deserialize<T>(Stream stream) =>
        (T)new XmlSerializer(typeof(T)).Deserialize(stream)!;
}
