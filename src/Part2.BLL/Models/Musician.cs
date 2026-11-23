using Lab33.BLL.Validation;

namespace Lab33.BLL.Models;

public class Musician : Person, ISkater
{
    public string MusicianId { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;

    public override string UniqueId => MusicianId;

    public override IEnumerable<(PersonField Field, string Value)> GetFields()
    {
        foreach (var field in base.GetFields())
            yield return field;
        yield return (PersonField.MusicianId, MusicianId);
        yield return (PersonField.Instrument, Instrument);
    }

    public string Skate() => $"Музикант {LastName} {FirstName} катається на ковзанах.";
}
