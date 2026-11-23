using System.Runtime.Serialization;
using System.Text.Json;
using Lab33.BLL.Exceptions;
using Lab33.BLL.Models;
using Lab33.BLL.Validation;
using Lab33.DAL;
using Lab33.DAL.Entities;
using Lab33.DAL.Providers;

namespace Lab33.BLL.Services;

public class EntityService
{
    private readonly EntityContext _context;

    public EntityService() : this(new EntityContext()) { }

    public EntityService(EntityContext context) => _context = context;

    // --- Стан бази даних ---
    public bool IsDatabaseOpen => _context.IsOpen;
    public string? CurrentFilePath => _context.FilePath;

    // --- Керування файлом бази даних ---
    public void CreateDatabase(string directory, string fileName, StorageFormat format) =>
        Guard(() => _context.Create(directory, fileName, ToDataFormat(format)));

    public void OpenDatabase(string directory, string fileName, StorageFormat format) =>
        Guard(() => _context.Open(directory, fileName, ToDataFormat(format)));

    public void CloseDatabase()
    {
        EnsureOpen();
        _context.Close();
    }

    public void DeleteDatabase()
    {
        EnsureOpen();
        Guard(_context.Delete);
    }

    // --- Додавання, видалення, отримання ---
    public void Add(Person person)
    {
        EnsureOpen();
        IReadOnlyList<string> errors = PersonValidator.Validate(person);
        if (errors.Count > 0)
            throw new InvalidPersonDataException(errors);
        if (Exists(person.UniqueId))
            throw new DuplicatePersonException(person.UniqueId);

        PersonEntity entity = EntityMapper.ToEntity(person);
        Guard(() =>
        {
            _context.Add(entity);
            try { _context.Update(); }
            catch { _context.Remove(entity); throw; }
        });
    }

    public void Remove(string id)
    {
        EnsureOpen();
        PersonEntity entity = FindEntity(id);
        Guard(() =>
        {
            _context.Remove(entity);
            try { _context.Update(); }
            catch { _context.Add(entity); throw; }
        });
    }

    public IReadOnlyList<Person> GetAll()
    {
        EnsureOpen();
        return _context.Entities.Select(EntityMapper.ToModel).ToList();
    }

    // --- Пошук ---
    public IReadOnlyList<Person> FindByLastName(string lastName)
    {
        EnsureOpen();
        return _context.Entities
            .Where(e => string.Equals(e.LastName, lastName.Trim(), StringComparison.OrdinalIgnoreCase))
            .Select(EntityMapper.ToModel).ToList();
    }

    public Person FindById(string id)
    {
        EnsureOpen();
        return EntityMapper.ToModel(FindEntity(id));
    }

    // --- Вміння ---
    public string Skate(string id)
    {
        Person person = FindById(id);
        return person is ISkater skater
            ? skater.Skate()
            : throw new SkillNotSupportedException("кататися на ковзанах");
    }

    // --- Студентки 1-го курсу в гуртожитку (дані читаються з файлу) ---
    public IReadOnlyList<Student> GetFirstYearFemaleDormStudents()
    {
        EnsureOpen();
        Guard(_context.Reload);
        return _context.Entities.Select(EntityMapper.ToModel).OfType<Student>()
            .Where(s => s.Course == 1 && s.Gender == Gender.Female && s.LivesInDormitory)
            .ToList();
    }

    // --- Перевірка введених даних ---
    public string? ValidateField(PersonField field, string? value) =>
        PersonValidator.CheckField(field, value);

    // --- Тестовий набір даних ---
    public int AddSampleData()
    {
        EnsureOpen();
        int added = 0;
        foreach (Person person in SampleData.Create().Where(p => !Exists(p.UniqueId)))
        {
            Add(person);
            added++;
        }
        return added;
    }

    // --- Допоміжні методи ---
    private bool Exists(string id) =>
        _context.Entities.Any(e => string.Equals(EntityMapper.GetId(e), id, StringComparison.OrdinalIgnoreCase));

    private PersonEntity FindEntity(string id) =>
        _context.Entities.FirstOrDefault(e =>
            string.Equals(EntityMapper.GetId(e), id.Trim(), StringComparison.OrdinalIgnoreCase))
        ?? throw new PersonNotFoundException(id);

    private void EnsureOpen()
    {
        if (!_context.IsOpen)
            throw new DatabaseNotOpenException();
    }

    private static DataFormat ToDataFormat(StorageFormat format) => format switch
    {
        StorageFormat.Binary => DataFormat.Binary,
        StorageFormat.Xml => DataFormat.Xml,
        StorageFormat.Json => DataFormat.Json,
        StorageFormat.Custom => DataFormat.Custom,
        _ => throw new ArgumentOutOfRangeException(nameof(format))
    };

    // --- Перетворення системних помилок DAL на винятки BLL ---
    private static void Guard(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SerializationException
            or InvalidOperationException or JsonException or FormatException or ArgumentException
            or InvalidCastException or System.Reflection.TargetInvocationException or NotSupportedException)
        {
            throw new StorageException($"Операція з файлом не виконана (файл пошкоджено, має інший формат або недоступний). Деталі: {ex.Message}", ex);
        }
    }
}
