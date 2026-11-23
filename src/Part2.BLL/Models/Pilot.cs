using Lab33.BLL.Validation;

namespace Lab33.BLL.Models;

public class Pilot : Person, ISkater
{
    public string PilotLicense { get; set; } = string.Empty;
    public int FlightHours { get; set; }

    public override string UniqueId => PilotLicense;

    public override IEnumerable<(PersonField Field, string Value)> GetFields()
    {
        foreach (var field in base.GetFields())
            yield return field;
        yield return (PersonField.PilotLicense, PilotLicense);
        yield return (PersonField.FlightHours, FlightHours.ToString());
    }

    public string Skate() => $"Пілот {LastName} {FirstName} катається на ковзанах.";
}
