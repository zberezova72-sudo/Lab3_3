using System.Globalization;

namespace Lab33.Part1.UI;

public static class ConsoleInput
{
    // --- Зчитування рядка (кінець вводу завершує програму) ---
    private static string Read() => Console.ReadLine() ?? throw new EndOfStreamException();

    // --- Рядок ---
    public static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            string? value = Read().Trim();
            if (!string.IsNullOrEmpty(value))
                return value;
            Console.WriteLine("Значення не може бути порожнім.");
        }
    }

    // --- Ціле число ---
    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write($"{prompt} [{min}..{max}]: ");
            if (int.TryParse(Read(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                && value >= min && value <= max)
                return value;
            Console.WriteLine("Некоректне число.");
        }
    }

    // --- Десяткове число ---
    public static decimal ReadDecimal(string prompt, decimal min)
    {
        while (true)
        {
            Console.Write($"{prompt} (>= {min}): ");
            string text = Read().Replace(',', '.');
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
                && value >= min)
                return value;
            Console.WriteLine("Некоректне число.");
        }
    }
}
