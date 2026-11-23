using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab33.DAL.Providers;

// Власний текстовий формат:
// ТипДаних НазваОб'єкта
// { "поле": "значення", ... };
public class CustomDataProvider<T> : DataProvider<T> where T : class
{
    private static readonly Regex BlockRegex =
        new(@"^(?<type>\w+)[ \t]+(?<name>\S+)\s*\{(?<body>.*?)\};", RegexOptions.Singleline | RegexOptions.Multiline);

    private static readonly Regex FieldRegex =
        new("\"(?<key>\\w+)\"\\s*:\\s*\"(?<value>(?:[^\"\\\\]|\\\\.)*)\"");

    private static readonly Type[] KnownTypes = typeof(T).Assembly.GetTypes()
        .Where(t => !t.IsAbstract && typeof(T).IsAssignableFrom(t)).ToArray();

    public override string Extension => ".txt";

    // --- Запис ---
    protected override void Serialize(Stream stream, List<T> items)
    {
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true);
        int index = 1;
        foreach (T item in items)
        {
            Type type = item.GetType();
            string fields = string.Join(",\n  ", GetProperties(type).Select(p =>
                $"\"{p.Name}\": \"{Escape(Convert.ToString(p.GetValue(item), CultureInfo.InvariantCulture))}\""));
            writer.WriteLine($"{type.Name} {type.Name}{index++}");
            writer.WriteLine($"{{ {fields}}};");
        }
    }

    // --- Читання ---
    protected override List<T> Deserialize(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true);
        string text = reader.ReadToEnd();
        var result = new List<T>();

        foreach (Match block in BlockRegex.Matches(text))
        {
            string typeName = block.Groups["type"].Value;
            Type type = KnownTypes.FirstOrDefault(t => t.Name == typeName)
                ?? throw new FormatException($"Невідомий тип даних: {typeName}.");
            object item = Activator.CreateInstance(type)!;

            foreach (Match field in FieldRegex.Matches(block.Groups["body"].Value))
            {
                PropertyInfo property = type.GetProperty(field.Groups["key"].Value)
                    ?? throw new FormatException($"Невідоме поле: {field.Groups["key"].Value}.");
                object? value = TypeDescriptor.GetConverter(property.PropertyType)
                    .ConvertFromInvariantString(Unescape(field.Groups["value"].Value));
                property.SetValue(item, value);
            }
            result.Add((T)item);
        }

        if (result.Count == 0 && text.Trim().Length > 0)
            throw new FormatException("Файл не відповідає користувацькому формату.");
        return result;

    }

    // --- Допоміжні методи ---
    private static PropertyInfo[] GetProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite).ToArray();

    private static string Escape(string? value) =>
        (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");

    private static string Unescape(string value) =>
        Regex.Replace(value, @"\\(.)", m => m.Groups[1].Value == "n" ? "\n" : m.Groups[1].Value);
}
