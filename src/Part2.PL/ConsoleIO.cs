using Lab33.BLL.Services;
using Lab33.BLL.Validation;

namespace Lab33.PL;

internal static class ConsoleIO
{
    // --- Зчитування рядка (кінець вводу завершує програму) ---
    private static string Read() => Console.ReadLine() ?? throw new EndOfStreamException();

    // --- Рядок ---
    public static string ReadLine(string prompt, string? defaultValue = null)
    {
        while (true)
        {
            Console.Write(defaultValue is null ? $"{prompt}: " : $"{prompt} [{defaultValue}]: ");
            string value = Read().Trim();
            if (value.Length > 0) return value;
            if (defaultValue is not null) return defaultValue;
            Console.WriteLine("Значення не може бути порожнім.");
        }
    }

    // --- Число з діапазону ---
    public static int ReadChoice(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write($"{prompt} [{min}..{max}]: ");
            if (int.TryParse(Read(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine("Некоректний вибір.");
        }
    }

    // --- Підтвердження ---
    public static bool Confirm(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (т/н): ");
            string answer = Read().Trim().ToLowerInvariant();
            if (answer is "т" or "так" or "y" or "yes") return true;
            if (answer is "н" or "ні" or "n" or "no") return false;
        }
    }

    // --- Поле з перевіркою правилами BLL (повторення до коректного значення) ---
    public static string ReadField(EntityService service, PersonField field, string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            string value = Read().Trim();
            string? error = service.ValidateField(field, value);
            if (error is null) return value;
            Console.WriteLine($"  Помилка: {error}");
        }
    }
}
