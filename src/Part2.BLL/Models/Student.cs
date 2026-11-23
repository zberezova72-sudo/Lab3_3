using Lab33.BLL.Validation;

namespace Lab33.BLL.Models;

public class Student : Person, ISkater
{
    public int Course { get; set; }
    public string StudentCard { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string Residence { get; set; } = string.Empty;

    public override string UniqueId => StudentCard;
    public bool LivesInDormitory => PersonValidator.IsDormitory(Residence);

    public override IEnumerable<(PersonField Field, string Value)> GetFields()
    {
        foreach (var field in base.GetFields())
            yield return field;
        yield return (PersonField.Course, Course.ToString());
        yield return (PersonField.StudentCard, StudentCard);
        yield return (PersonField.Residence, Residence);
    }

    public string Skate() => $"Студент {LastName} {FirstName} катається на ковзанах.";
}
