namespace Lab33.Part1.Models;

[Serializable]
public class Product : IEquatable<Product>
{
    // --- Властивості ---
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // --- Конструктори ---
    public Product() { }

    public Product(int code, string name, string manufacturer, decimal price, int quantity)
    {
        Code = code;
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        Quantity = quantity;
    }

    // --- Методи ---
    public decimal GetTotalCost() => Price * Quantity;

    public void IncreasePrice(decimal percent)
    {
        if (percent < 0)
            throw new ArgumentOutOfRangeException(nameof(percent), "Відсоток не може бути від'ємним.");
        Price = Math.Round(Price * (1 + percent / 100m), 2);
    }

    public string GetInfo() =>
        $"Код: {Code}, назва: {Name}, виробник: {Manufacturer}, " +
        $"вартість: {Price:F2}, кількість у партії: {Quantity}, загальна вартість партії: {GetTotalCost():F2}";

    // --- Порівняння ---
    public bool Equals(Product? other) =>
        other is not null && Code == other.Code && Name == other.Name &&
        Manufacturer == other.Manufacturer && Price == other.Price && Quantity == other.Quantity;

    public override bool Equals(object? obj) => Equals(obj as Product);

    public override int GetHashCode() => HashCode.Combine(Code, Name, Manufacturer, Price, Quantity);
}
