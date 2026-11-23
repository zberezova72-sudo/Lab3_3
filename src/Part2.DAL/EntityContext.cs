using Lab33.DAL.Entities;
using Lab33.DAL.Providers;

namespace Lab33.DAL;

public class EntityContext
{
    private readonly List<PersonEntity> _entities = new();
    private DataProvider<PersonEntity>? _provider;

    // --- Стан ---
    public bool IsOpen => _provider is not null;
    public string? FilePath { get; private set; }
    public IReadOnlyList<PersonEntity> Entities => _entities;

    // --- Створення, відкриття, закриття, видалення файлу ---
    public void Create(string directory, string fileName, DataFormat format)
    {
        var provider = DataProviderFactory.Create<PersonEntity>(format);
        string path = BuildPath(directory, fileName, provider);
        if (File.Exists(path))
            throw new IOException($"Файл «{path}» уже існує.");

        string? folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
            Directory.CreateDirectory(folder);
        provider.Save(path, new List<PersonEntity>());

        Attach(provider, path, new List<PersonEntity>());
    }

    public void Open(string directory, string fileName, DataFormat format)
    {
        var provider = DataProviderFactory.Create<PersonEntity>(format);
        string path = BuildPath(directory, fileName, provider);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Файл «{path}» не знайдено.");

        Attach(provider, path, provider.Load(path));
    }

    public void Close()
    {
        _entities.Clear();
        _provider = null;
        FilePath = null;
    }

    public void Delete()
    {
        EnsureOpen();
        string path = FilePath!;
        Close();
        File.Delete(path);
    }

    // --- Оновлення даних ---
    public void Add(PersonEntity entity)
    {
        EnsureOpen();
        _entities.Add(entity);
    }

    public bool Remove(PersonEntity entity)
    {
        EnsureOpen();
        return _entities.Remove(entity);
    }

    public void Update()
    {
        EnsureOpen();
        _provider!.Save(FilePath!, _entities);
    }

    public void Reload()
    {
        EnsureOpen();
        List<PersonEntity> loaded = _provider!.Load(FilePath!);
        _entities.Clear();
        _entities.AddRange(loaded);
    }

    // --- Допоміжні методи ---
    private void Attach(DataProvider<PersonEntity> provider, string path, List<PersonEntity> data)
    {
        _provider = provider;
        FilePath = path;
        _entities.Clear();
        _entities.AddRange(data);
    }

    private static string BuildPath(string directory, string fileName, DataProvider<PersonEntity> provider)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Ім'я файлу не може бути порожнім.");
        if (!fileName.EndsWith(provider.Extension, StringComparison.OrdinalIgnoreCase))
            fileName += provider.Extension;
        return Path.Combine(directory ?? string.Empty, fileName);
    }

    private void EnsureOpen()
    {
        if (!IsOpen)
            throw new InvalidOperationException("Файл бази даних не відкрито.");
    }
}
