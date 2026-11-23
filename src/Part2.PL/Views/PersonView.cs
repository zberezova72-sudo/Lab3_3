namespace Lab33.PL.Views;

public enum GenderView
{
    Male,
    Female
}

public abstract class PersonView
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public abstract string Title { get; }
    public abstract string UniqueId { get; }
    protected abstract string Details { get; }

    public override string ToString() =>
        $"[{Title}] {LastName} {FirstName}; ідентифікатор: {UniqueId}; {Details}";
}

public class StudentView : PersonView
{
    public int Course { get; set; }
    public string StudentCard { get; set; } = string.Empty;
    public GenderView Gender { get; set; }
    public string Residence { get; set; } = string.Empty;

    public override string Title => "Студент";
    public override string UniqueId => StudentCard;
    protected override string Details =>
        $"курс: {Course}; стать: {(Gender == GenderView.Female ? "жіноча" : "чоловіча")}; проживання: {Residence}";
}

public class MusicianView : PersonView
{
    public string MusicianId { get; set; } = string.Empty;
    public string Instrument { get; set; } = string.Empty;

    public override string Title => "Музикант";
    public override string UniqueId => MusicianId;
    protected override string Details => $"інструмент: {Instrument}";
}

public class PilotView : PersonView
{
    public string PilotLicense { get; set; } = string.Empty;
    public int FlightHours { get; set; }

    public override string Title => "Пілот";
    public override string UniqueId => PilotLicense;
    protected override string Details => $"годин нальоту: {FlightHours}";
}
