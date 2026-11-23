using Lab33.BLL.Validation;

namespace Lab33.BLL.Models;

public abstract class Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // --- Унікальний ідентифікатор особи ---
    public abstract string UniqueId { get; }

    // --- Поля для перевірки за правилами ---
    public virtual IEnumerable<(PersonField Field, string Value)> GetFields()
    {
        yield return (PersonField.FirstName, FirstName);
        yield return (PersonField.LastName, LastName);
    }
}
