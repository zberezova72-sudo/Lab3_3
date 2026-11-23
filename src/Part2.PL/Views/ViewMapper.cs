using Lab33.BLL.Models;
using Lab33.PL.Views;

namespace Lab33.PL.Views;

internal static class ViewMapper
{
    // --- Модель BLL -> подання PL ---
    public static PersonView ToView(Person person) => person switch
    {
        Student s => new StudentView
        {
            FirstName = s.FirstName, LastName = s.LastName, Course = s.Course, StudentCard = s.StudentCard,
            Residence = s.Residence, Gender = s.Gender == Gender.Female ? GenderView.Female : GenderView.Male
        },
        Musician m => new MusicianView
        {
            FirstName = m.FirstName, LastName = m.LastName, MusicianId = m.MusicianId, Instrument = m.Instrument
        },
        Pilot p => new PilotView
        {
            FirstName = p.FirstName, LastName = p.LastName, PilotLicense = p.PilotLicense, FlightHours = p.FlightHours
        },
        _ => throw new NotSupportedException($"Тип {person.GetType().Name} не підтримується.")
    };

    // --- Подання PL -> модель BLL ---
    public static Person ToModel(PersonView view) => view switch
    {
        StudentView s => new Student
        {
            FirstName = s.FirstName, LastName = s.LastName, Course = s.Course, StudentCard = s.StudentCard,
            Residence = s.Residence, Gender = s.Gender == GenderView.Female ? Gender.Female : Gender.Male
        },
        MusicianView m => new Musician
        {
            FirstName = m.FirstName, LastName = m.LastName, MusicianId = m.MusicianId, Instrument = m.Instrument
        },
        PilotView p => new Pilot
        {
            FirstName = p.FirstName, LastName = p.LastName, PilotLicense = p.PilotLicense, FlightHours = p.FlightHours
        },
        _ => throw new NotSupportedException($"Тип {view.GetType().Name} не підтримується.")
    };
}
