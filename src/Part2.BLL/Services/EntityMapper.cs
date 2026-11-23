using Lab33.BLL.Models;
using Lab33.DAL.Entities;

namespace Lab33.BLL.Services;

internal static class EntityMapper
{
    // --- Модель BLL -> сутність DAL ---
    public static PersonEntity ToEntity(Person person) => person switch
    {
        Student s => new StudentEntity
        {
            FirstName = s.FirstName, LastName = s.LastName, Course = s.Course,
            StudentCard = s.StudentCard, Residence = s.Residence,
            Gender = s.Gender == Gender.Female ? GenderEntity.Female : GenderEntity.Male
        },
        Musician m => new MusicianEntity
        {
            FirstName = m.FirstName, LastName = m.LastName,
            MusicianId = m.MusicianId, Instrument = m.Instrument
        },
        Pilot p => new PilotEntity
        {
            FirstName = p.FirstName, LastName = p.LastName,
            PilotLicense = p.PilotLicense, FlightHours = p.FlightHours
        },
        _ => throw new NotSupportedException($"Тип {person.GetType().Name} не підтримується.")
    };

    // --- Сутність DAL -> модель BLL ---
    public static Person ToModel(PersonEntity entity) => entity switch
    {
        StudentEntity s => new Student
        {
            FirstName = s.FirstName, LastName = s.LastName, Course = s.Course,
            StudentCard = s.StudentCard, Residence = s.Residence,
            Gender = s.Gender == GenderEntity.Female ? Gender.Female : Gender.Male
        },
        MusicianEntity m => new Musician
        {
            FirstName = m.FirstName, LastName = m.LastName,
            MusicianId = m.MusicianId, Instrument = m.Instrument
        },
        PilotEntity p => new Pilot
        {
            FirstName = p.FirstName, LastName = p.LastName,
            PilotLicense = p.PilotLicense, FlightHours = p.FlightHours
        },
        _ => throw new NotSupportedException($"Тип {entity.GetType().Name} не підтримується.")
    };

    // --- Ідентифікатор сутності ---
    public static string GetId(PersonEntity entity) => ToModel(entity).UniqueId;
}
