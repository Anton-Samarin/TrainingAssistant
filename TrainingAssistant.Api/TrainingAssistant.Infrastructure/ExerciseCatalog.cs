namespace TrainingAssistant.Infrastructure;

/// <summary>
/// Каталог упражнений для UI замены (синхронизирован с пулами ML)
/// </summary>
public static class ExerciseCatalog
{
    private static readonly Dictionary<string, string[]> Pools = new(StringComparer.OrdinalIgnoreCase)
    {
        ["upper"] =
        [
            "Отжимания", "Отжимания с колен", "Жим гантелей лёжа", "Жим гантелей сидя",
            "Тяга гантели в наклоне", "Разведения гантелей", "Сгибания рук с гантелями",
            "Подтягивания", "Жим в тренажёре"
        ],
        ["lower"] =
        [
            "Приседания с собственным весом", "Гоблет-присед", "Ягодичный мост",
            "Приседания у стены", "Выпады", "Подъёмы на носки стоя", "Жим ногами в тренажёре"
        ],
        ["full"] =
        [
            "Планка", "Боковая планка", "Шаги на месте с высоким коленом",
            "Шаги на месте", "Скакалка", "Велотренажёр", "Эллипс", "Махи гантелью"
        ],
        ["safe"] =
        [
            "Планка", "Боковая планка", "Ягодичный мост", "Приседания у стены",
            "Шаги на месте", "Велосипед лёжа", "Жим гантелей сидя", "Растяжка задней поверхности бедра"
        ]
    };

    public static IReadOnlyList<string> GetPool(string poolKey) =>
        Pools.TryGetValue(poolKey, out var items) ? items : Pools["safe"];

    public static IReadOnlyList<string> GetAlternatives(string poolKey, string excludeName, int max = 12)
    {
        return GetPool(poolKey)
            .Where(n => !string.Equals(n, excludeName, StringComparison.OrdinalIgnoreCase))
            .Take(max)
            .ToList();
    }

    public static string GuessPoolForExercise(string name)
    {
        foreach (var (key, items) in Pools)
        {
            if (items.Any(i => string.Equals(i, name, StringComparison.OrdinalIgnoreCase)))
                return key;
        }
        return "safe";
    }
}
