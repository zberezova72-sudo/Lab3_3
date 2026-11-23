namespace Lab33.BLL.Exceptions;

// Базовий виняток предметної області
public class DomainException : Exception
{
    public DomainException(string message, Exception? inner = null) : base(message, inner) { }
}

public class InvalidPersonDataException : DomainException
{
    public IReadOnlyList<string> Errors { get; }

    public InvalidPersonDataException(IReadOnlyList<string> errors)
        : base("Дані особи некоректні: " + string.Join("; ", errors)) => Errors = errors;
}

public class DuplicatePersonException : DomainException
{
    public DuplicatePersonException(string id)
        : base($"Особа з ідентифікатором «{id}» уже існує.") { }
}

public class PersonNotFoundException : DomainException
{
    public PersonNotFoundException(string id)
        : base($"Особу з ідентифікатором «{id}» не знайдено.") { }
}

public class DatabaseNotOpenException : DomainException
{
    public DatabaseNotOpenException()
        : base("База даних не відкрита. Спочатку створіть або відкрийте файл.") { }
}

public class SkillNotSupportedException : DomainException
{
    public SkillNotSupportedException(string skill)
        : base($"Ця особа не вміє: {skill}.") { }
}

public class StorageException : DomainException
{
    public StorageException(string message, Exception inner) : base(message, inner) { }
}
