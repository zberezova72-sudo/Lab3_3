namespace Lab33.DAL.Entities;

[Serializable]
public class StudentEntity : PersonEntity
{
    public int Course { get; set; }
    public string StudentCard { get; set; } = string.Empty;
    public GenderEntity Gender { get; set; }
    public string Residence { get; set; } = string.Empty;
}
