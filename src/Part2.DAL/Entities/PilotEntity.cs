namespace Lab33.DAL.Entities;

[Serializable]
public class PilotEntity : PersonEntity
{
    public string PilotLicense { get; set; } = string.Empty;
    public int FlightHours { get; set; }
}
