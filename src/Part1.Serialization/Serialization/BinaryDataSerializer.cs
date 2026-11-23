using System.Runtime.Serialization.Formatters.Binary;

namespace Lab33.Part1.Serialization;

public class BinaryDataSerializer : DataSerializer
{
    public override string Name => "Бінарна";
    public override string Extension => ".bin";

    protected override void Serialize<T>(Stream stream, T data) =>
        new BinaryFormatter().Serialize(stream, data!);

    protected override T Deserialize<T>(Stream stream) =>
        (T)new BinaryFormatter().Deserialize(stream);
}
