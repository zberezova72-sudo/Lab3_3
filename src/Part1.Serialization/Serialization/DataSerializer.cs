namespace Lab33.Part1.Serialization;

public abstract class DataSerializer
{
    // --- Опис формату ---
    public abstract string Name { get; }
    public abstract string Extension { get; }

    // --- Робота з файлом ---
    public void SaveToFile<T>(string path, T data)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        Serialize(stream, data);
    }

    public T LoadFromFile<T>(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return Deserialize<T>(stream);
    }

    // --- Реалізація конкретного формату ---
    protected abstract void Serialize<T>(Stream stream, T data);
    protected abstract T Deserialize<T>(Stream stream);
}
