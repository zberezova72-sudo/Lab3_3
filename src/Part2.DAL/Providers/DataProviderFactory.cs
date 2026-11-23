namespace Lab33.DAL.Providers;

public static class DataProviderFactory
{
    public static DataProvider<T> Create<T>(DataFormat format) where T : class => format switch
    {
        DataFormat.Binary => new BinaryDataProvider<T>(),
        DataFormat.Xml => new XmlDataProvider<T>(),
        DataFormat.Json => new JsonDataProvider<T>(),
        DataFormat.Custom => new CustomDataProvider<T>(),
        _ => throw new ArgumentOutOfRangeException(nameof(format))
    };
}
