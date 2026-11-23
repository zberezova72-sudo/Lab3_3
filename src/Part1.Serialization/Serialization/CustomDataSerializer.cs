using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Lab33.Part1.Models;

namespace Lab33.Part1.Serialization;

public class CustomDataSerializer : DataSerializer
{
    public override string Name => "Користувацька";
    public override string Extension => ".custom";

    // --- Форматер із підміною стандартної логіки для Product ---
    private static BinaryFormatter CreateFormatter()
    {
        var context = new StreamingContext(StreamingContextStates.All);
        var selector = new SurrogateSelector();
        selector.AddSurrogate(typeof(Product), context, new ProductSurrogate());
        return new BinaryFormatter(selector, context);
    }

    protected override void Serialize<T>(Stream stream, T data) =>
        CreateFormatter().Serialize(stream, data!);

    protected override T Deserialize<T>(Stream stream) =>
        (T)CreateFormatter().Deserialize(stream);

    // --- Власні правила запису та відновлення полів ---
    private sealed class ProductSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var product = (Product)obj;
            info.AddValue("product_code", product.Code);
            info.AddValue("title", product.Name);
            info.AddValue("vendor", product.Manufacturer);
            info.AddValue("price_text", product.Price.ToString(CultureInfo.InvariantCulture));
            info.AddValue("amount", product.Quantity);
            info.AddValue("batch_total", product.GetTotalCost().ToString(CultureInfo.InvariantCulture));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
            ISurrogateSelector? selector)
        {
            var product = (Product)obj;
            product.Code = info.GetInt32("product_code");
            product.Name = info.GetString("title") ?? string.Empty;
            product.Manufacturer = info.GetString("vendor") ?? string.Empty;
            product.Price = decimal.Parse(info.GetString("price_text")!, CultureInfo.InvariantCulture);
            product.Quantity = info.GetInt32("amount");
            return product;
        }
    }
}
