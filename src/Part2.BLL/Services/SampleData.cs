using Lab33.BLL.Models;

namespace Lab33.BLL.Services;

// Готовий набір даних для перевірки роботи програми
internal static class SampleData
{
    public static IEnumerable<Person> Create() => new Person[]
    {
        new Student { FirstName = "Олена", LastName = "Коваленко", Course = 1, StudentCard = "KB100001", Gender = Gender.Female, Residence = "5.312" },
        new Student { FirstName = "Марія", LastName = "Шевченко", Course = 1, StudentCard = "KB100002", Gender = Gender.Female, Residence = "2.104" },
        new Student { FirstName = "Софія", LastName = "Бондаренко", Course = 1, StudentCard = "KB100003", Gender = Gender.Female, Residence = "Київ, вул. Польова 12" },
        new Student { FirstName = "Андрій", LastName = "Мельник", Course = 1, StudentCard = "KB100004", Gender = Gender.Male, Residence = "5.201" },
        new Student { FirstName = "Ірина", LastName = "Ткаченко", Course = 3, StudentCard = "KB300005", Gender = Gender.Female, Residence = "3.415" },
        new Musician { FirstName = "Тарас", LastName = "Лисенко", MusicianId = "MZ0001", Instrument = "Скрипка" },
        new Musician { FirstName = "Оксана", LastName = "Гнатюк", MusicianId = "MZ0002", Instrument = "Фортепіано" },
        new Pilot { FirstName = "Василь", LastName = "Савченко", PilotLicense = "PL10001", FlightHours = 1250 },
        new Pilot { FirstName = "Наталія", LastName = "Кравчук", PilotLicense = "PL10002", FlightHours = 640 }
    };
}
