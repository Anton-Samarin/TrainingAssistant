using System.Text.Json;

namespace TrainingAssistant.Infrastructure.Helpers;

/// <summary>
/// Сериализация списков строк в JSON для EF
/// </summary>
public static class JsonListHelper
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>
    /// Сохраняет список строк в JSON для поля профиля
    /// </summary>
    /// <param name="items">Оборудование или травмы</param>
    /// <returns>JSON-массив строк</returns>
    public static string Serialize(IEnumerable<string> items) =>
        JsonSerializer.Serialize(items.Select(x => x.Trim().ToLowerInvariant()).Where(x => x.Length > 0).Distinct(), Options);

    /// <summary>
    /// Читает список строк из JSON, при ошибке отдаёт пустой список
    /// </summary>
    /// <param name="json">Сохранённое значение из БД</param>
    /// <returns>Список строк</returns>
    public static List<string> Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, Options) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
