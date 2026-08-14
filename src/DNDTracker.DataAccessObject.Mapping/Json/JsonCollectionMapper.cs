using System.Text.Json;

namespace DNDTracker.DataAccessObject.Mapping.Json;

internal static class JsonCollectionMapper
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static string Serialize<T>(IEnumerable<T> value)
    {
        return JsonSerializer.Serialize(value, Options);
    }

    public static IReadOnlyCollection<T> DeserializeCollection<T>(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return [];

        return JsonSerializer.Deserialize<List<T>>(value, Options) ?? [];
    }

    public static T? Deserialize<T>(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        return JsonSerializer.Deserialize<T>(value, Options);
    }

    public static string SerializeObject<T>(T value)
    {
        return JsonSerializer.Serialize(value, Options);
    }
}
