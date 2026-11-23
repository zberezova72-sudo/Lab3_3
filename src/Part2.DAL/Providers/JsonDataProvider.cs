using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lab33.DAL.Providers;

public class JsonDataProvider<T> : DataProvider<T> where T : class
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public override string Extension => ".json";

    protected override void Serialize(Stream stream, List<T> items) =>
        JsonSerializer.Serialize(stream, items, Options);

    protected override List<T> Deserialize(Stream stream) =>
        JsonSerializer.Deserialize<List<T>>(stream, Options)
        ?? throw new InvalidDataException("Файл JSON не містить даних.");
}
