using System.Text;
using Lab33.Part1.Models;
using Lab33.Part1.Serialization;

namespace Lab33.Part1.UI;

public class ConsoleMenu
{
    private const int ProductsCount = 5;
    private const string DataDirectory = "Data";

    private readonly DataSerializer[] _serializers =
    {
        new BinaryDataSerializer(),
        new CustomDataSerializer(),
        new XmlDataSerializer(),
        new JsonDataSerializer()
    };

    private Product[] _products = Array.Empty<Product>();
    private Product[] _restored = Array.Empty<Product>();

    // --- Головний цикл ---
    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            PrintMenu();
            int choice = ConsoleInput.ReadInt("Ваш вибір", 0, 9);
            if (choice == 0)
                return;

            try
            {
                Execute(choice);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                or System.Runtime.Serialization.SerializationException or InvalidOperationException
                or System.Text.Json.JsonException or InvalidDataException or ArgumentException)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
            catch (EndOfStreamException)
            {
                return;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Частина 1. Серіалізація (клас Товар) ===");
        Console.WriteLine("1. Створити масив із 5 товарів (ручне введення)");
        Console.WriteLine("2. Заповнити масив готовим набором даних");
        Console.WriteLine("3. Вивести масив");
        Console.WriteLine("4. Загальна вартість партії");
        Console.WriteLine("5. Збільшити вартість на відсоток");
        Console.WriteLine("6. Серіалізувати масив у файл");
        Console.WriteLine("7. Десеріалізувати у новий масив і порівняти");
        Console.WriteLine("8. Серіалізація колекції List та порівняння з масивом");
        Console.WriteLine("9. Порівняння всіх видів серіалізації");
        Console.WriteLine("0. Вихід");
    }

    private void Execute(int choice)
    {
        switch (choice)
        {
            case 1: CreateManually(); break;
            case 2: FillSample(); break;
            case 3: Print("Поточний масив", _products); break;
            case 4: ShowTotalCost(); break;
            case 5: IncreasePrices(); break;
            case 6: SerializeArray(); break;
            case 7: DeserializeArray(); break;
            case 8: CompareWithCollection(); break;
            case 9: CompareAllFormats(); break;
        }
    }

    // --- Створення даних ---
    private void CreateManually()
    {
        _products = new Product[ProductsCount];
        for (int i = 0; i < ProductsCount; i++)
        {
            Console.WriteLine($"Товар {i + 1} з {ProductsCount}:");
            _products[i] = new Product(
                ConsoleInput.ReadInt("  Код", 1, int.MaxValue),
                ConsoleInput.ReadString("  Назва"),
                ConsoleInput.ReadString("  Виробник"),
                ConsoleInput.ReadDecimal("  Вартість", 0.01m),
                ConsoleInput.ReadInt("  Кількість у партії", 0, int.MaxValue));
        }
        Console.WriteLine("Масив створено.");
    }

    private void FillSample()
    {
        _products = new[]
        {
            new Product(101, "Ноутбук", "Lenovo", 25999.90m, 12),
            new Product(102, "Смартфон", "Samsung", 14500m, 30),
            new Product(103, "Навушники", "Sony", 3200.50m, 45),
            new Product(104, "Монітор", "Dell", 8900m, 8),
            new Product(105, "Клавіатура", "Logitech", 1750.75m, 60)
        };
        Console.WriteLine("Масив заповнено.");
    }

    // --- Методи класу Product ---
    private void ShowTotalCost()
    {
        if (!HasProducts()) return;
        decimal sum = 0;
        foreach (Product product in _products)
        {
            Console.WriteLine($"{product.Name}: {product.GetTotalCost():F2}");
            sum += product.GetTotalCost();
        }
        Console.WriteLine($"Разом за всіма партіями: {sum:F2}");
    }

    private void IncreasePrices()
    {
        if (!HasProducts()) return;
        Print("Товари", _products);
        int number = ConsoleInput.ReadInt("Номер товару (0 - усі)", 0, _products.Length);
        decimal percent = ConsoleInput.ReadDecimal("Відсоток збільшення", 0);

        if (number == 0)
            foreach (Product product in _products)
                product.IncreasePrice(percent);
        else
            _products[number - 1].IncreasePrice(percent);

        Print("Після зміни вартості", _products);
    }

    // --- Серіалізація масиву ---
    private void SerializeArray()
    {
        if (!HasProducts()) return;
        DataSerializer serializer = ChooseSerializer();
        string path = ArrayPath(serializer);
        serializer.SaveToFile(path, _products);
        Console.WriteLine($"Масив серіалізовано ({serializer.Name}): {path}, {FileSize(path)} байт.");
    }

    private void DeserializeArray()
    {
        DataSerializer serializer = ChooseSerializer();
        string path = ArrayPath(serializer);
        _restored = serializer.LoadFromFile<Product[]>(path);
        Print("Відновлений масив", _restored);
        Console.WriteLine($"Збігається з вихідним масивом: {SameContent(_products, _restored)}");
        Console.WriteLine($"Це інший об'єкт масиву: {!ReferenceEquals(_products, _restored)}");
    }

    // --- Колекція та порівняння з масивом ---
    private void CompareWithCollection()
    {
        if (!HasProducts()) return;
        DataSerializer serializer = ChooseSerializer();
        var list = new List<Product>(_products);
        string arrayPath = ArrayPath(serializer);
        string listPath = Path.Combine(DataDirectory, $"products_list{serializer.Extension}");

        serializer.SaveToFile(arrayPath, _products);
        serializer.SaveToFile(listPath, list);
        var restoredList = serializer.LoadFromFile<List<Product>>(listPath);

        Print("Відновлена колекція List<Product>", restoredList);
        Console.WriteLine($"Розмір файлу масиву: {FileSize(arrayPath)} байт");
        Console.WriteLine($"Розмір файлу колекції: {FileSize(listPath)} байт");
        Console.WriteLine($"Файли однакові за вмістом: {FilesEqual(arrayPath, listPath)}");
        Console.WriteLine($"Елементи колекції збігаються з масивом: {SameContent(_products, restoredList)}");
    }

    private void CompareAllFormats()
    {
        if (!HasProducts()) return;
        Console.WriteLine($"{"Формат",-15}{"Файл",-32}{"Розмір, байт",-14}Відновлено без втрат");
        foreach (DataSerializer serializer in _serializers)
        {
            string path = ArrayPath(serializer);
            serializer.SaveToFile(path, _products);
            var restored = serializer.LoadFromFile<Product[]>(path);
            Console.WriteLine($"{serializer.Name,-15}{path,-32}{FileSize(path),-14}{SameContent(_products, restored)}");
        }
    }

    // --- Допоміжні методи ---
    private DataSerializer ChooseSerializer()
    {
        Console.WriteLine("Вид серіалізації:");
        for (int i = 0; i < _serializers.Length; i++)
            Console.WriteLine($"  {i + 1}. {_serializers[i].Name} ({_serializers[i].Extension})");
        return _serializers[ConsoleInput.ReadInt("Вибір", 1, _serializers.Length) - 1];
    }

    private bool HasProducts()
    {
        if (_products.Length > 0) return true;
        Console.WriteLine("Спочатку створіть масив (пункт 1 або 2).");
        return false;
    }

    private static string ArrayPath(DataSerializer serializer) =>
        Path.Combine(DataDirectory, $"products{serializer.Extension}");

    private static long FileSize(string path) => new FileInfo(path).Length;

    private static bool SameContent(IEnumerable<Product> first, IEnumerable<Product> second) =>
        first.SequenceEqual(second);

    private static bool FilesEqual(string first, string second) =>
        File.ReadAllBytes(first).AsSpan().SequenceEqual(File.ReadAllBytes(second));

    private static void Print(string title, IEnumerable<Product> products)
    {
        Console.WriteLine($"--- {title} ---");
        int number = 1;
        foreach (Product product in products)
            Console.WriteLine($"{number++}. {product.GetInfo()}");
        if (number == 1)
            Console.WriteLine("(порожньо)");
    }
}
