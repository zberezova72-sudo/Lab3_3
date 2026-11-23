namespace Lab33.DAL.Entities;

[Serializable]
public class MusicianEntity : PersonEntity
{
    public string MusicianId { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;
}
