namespace Lab33.DAL.Providers;

public abstract class DataProvider<T> where T : class
{
    // --- Розширення файлу формату ---
    public abstract string Extension { get; }

    // --- Робота з файлом ---
    public void Save(string path, List<T> items)
    {
        using var buffer = new MemoryStream();
        Serialize(buffer, items);
        File.WriteAllBytes(path, buffer.ToArray());
    }

    public List<T> Load(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return Deserialize(stream);
    }

    // --- Реалізація конкретного формату ---
    protected abstract void Serialize(Stream stream, List<T> items);
    protected abstract List<T> Deserialize(Stream stream);
}
