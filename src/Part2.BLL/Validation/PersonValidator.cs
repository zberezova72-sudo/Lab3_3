using System.Text.RegularExpressions;
using Lab33.BLL.Models;

namespace Lab33.BLL.Validation;

public static class PersonValidator
{
    private const string Letters = @"A-Za-zА-Яа-яІіЇїЄєҐґ";

    private static readonly Regex DormitoryRegex = new(@"^\d{1,2}\.\d{1,3}$");

    // --- Правила: регулярний вираз і повідомлення про помилку ---
    private static readonly Dictionary<PersonField, (Regex Pattern, string Message)> Rules = new()
    {
        [PersonField.FirstName] = (new($"^[{Letters}][{Letters}'’\\- ]{{0,29}}$"),
            "допускаються лише літери, пробіл, дефіс та апостроф (до 30 символів)"),
        [PersonField.LastName] = (new($"^[{Letters}][{Letters}'’\\- ]{{0,29}}$"),
            "допускаються лише літери, пробіл, дефіс та апостроф (до 30 символів)"),
        [PersonField.Course] = (new(@"^[1-6]$"),
            "курс — число від 1 до 6"),
        [PersonField.StudentCard] = (new(@"^[A-Z]{2}\d{6}$"),
            "формат студентського квитка: дві латинські літери та 6 цифр, напр. KB123456"),
        [PersonField.Residence] = (new($@"^(\d{{1,2}}\.\d{{1,3}}|(?=.*[{Letters}])[{Letters}\d .,'’\-]{{2,60}})$"),
            "гуртожиток — «номер.кімната», напр. 5.312; інакше — адреса (літери, цифри)"),
        [PersonField.MusicianId] = (new(@"^MZ\d{4}$"),
            "формат посвідчення музиканта: MZ та 4 цифри, напр. MZ0001"),
        [PersonField.Instrument] = (new($"^[{Letters}][{Letters}'’\\- ]{{1,29}}$"),
            "допускаються лише літери (від 2 до 30 символів)"),
        [PersonField.PilotLicense] = (new(@"^PL\d{5}$"),
            "формат посвідчення пілота: PL та 5 цифр, напр. PL12345"),
        [PersonField.FlightHours] = (new(@"^\d{1,5}$"),
            "кількість годин — ціле невід'ємне число")
    };

    // --- Перевірка одного поля (повертає текст помилки або null) ---
    public static string? CheckField(PersonField field, string? value)
    {
        var (pattern, message) = Rules[field];
        return value is not null && pattern.IsMatch(value) ? null : $"{field}: {message}";
    }

    // --- Перевірка всієї особи ---
    public static IReadOnlyList<string> Validate(Person person) =>
        person.GetFields()
            .Select(f => CheckField(f.Field, f.Value))
            .Where(error => error is not null)
            .Select(error => error!)
            .ToList();

    // --- Чи вказано гуртожиток у форматі «номер.кімната» ---
    public static bool IsDormitory(string? residence) =>
        residence is not null && DormitoryRegex.IsMatch(residence);
}
