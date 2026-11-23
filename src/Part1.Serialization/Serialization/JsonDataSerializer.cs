using System.Text.Json;

namespace Lab33.Part1.Serialization;

public class JsonDataSerializer : DataSerializer
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public override string Name => "JSON";
    public override string Extension => ".json";

    protected override void Serialize<T>(Stream stream, T data) =>
        JsonSerializer.Serialize(stream, data, Options);

    protected override T Deserialize<T>(Stream stream) =>
        JsonSerializer.Deserialize<T>(stream, Options)
        ?? throw new InvalidDataException("Файл JSON не містить даних.");
}
