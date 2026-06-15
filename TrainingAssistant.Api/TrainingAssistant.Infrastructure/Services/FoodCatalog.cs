using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Infrastructure.Services;

internal enum FoodRole
{
    Protein,
    Carb,
    Vegetable,
    Fruit,
    Dairy,
    Condiment
}

internal sealed record MealSlot(FoodRole Role, decimal CalorieShare, bool Required = true);

/// <summary>
/// Роли продуктов и шаблоны приёмов пищи
/// </summary>
internal static class FoodCatalog
{
    private static readonly Dictionary<Guid, FoodRole> Roles = new()
    {
        { Guid.Parse("11111111-1111-1111-1111-111111111101"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111102"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111103"), FoodRole.Dairy },
        { Guid.Parse("11111111-1111-1111-1111-111111111104"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111105"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111106"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111107"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111108"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111109"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111110"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111111"), FoodRole.Fruit },
        { Guid.Parse("11111111-1111-1111-1111-111111111112"), FoodRole.Fruit },
        { Guid.Parse("11111111-1111-1111-1111-111111111113"), FoodRole.Dairy },
        { Guid.Parse("11111111-1111-1111-1111-111111111114"), FoodRole.Condiment },
        { Guid.Parse("11111111-1111-1111-1111-111111111115"), FoodRole.Condiment },
        { Guid.Parse("11111111-1111-1111-1111-111111111116"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111117"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111118"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111119"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111120"), FoodRole.Protein },
        { Guid.Parse("11111111-1111-1111-1111-111111111121"), FoodRole.Dairy },
        { Guid.Parse("11111111-1111-1111-1111-111111111122"), FoodRole.Dairy },
        { Guid.Parse("11111111-1111-1111-1111-111111111123"), FoodRole.Dairy },
        { Guid.Parse("11111111-1111-1111-1111-111111111124"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111125"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111126"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111127"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111128"), FoodRole.Carb },
        { Guid.Parse("11111111-1111-1111-1111-111111111129"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111130"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111131"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111132"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111133"), FoodRole.Vegetable },
        { Guid.Parse("11111111-1111-1111-1111-111111111134"), FoodRole.Fruit },
        { Guid.Parse("11111111-1111-1111-1111-111111111135"), FoodRole.Fruit },
        { Guid.Parse("11111111-1111-1111-1111-111111111136"), FoodRole.Fruit },
        { Guid.Parse("11111111-1111-1111-1111-111111111137"), FoodRole.Condiment },
        { Guid.Parse("11111111-1111-1111-1111-111111111138"), FoodRole.Condiment },
        { Guid.Parse("11111111-1111-1111-1111-111111111139"), FoodRole.Condiment },
        { Guid.Parse("11111111-1111-1111-1111-111111111140"), FoodRole.Carb },
    };

    private static readonly IReadOnlyDictionary<MealType, MealSlot[]> MealTemplates =
        new Dictionary<MealType, MealSlot[]>
        {
            [MealType.Breakfast] =
            [
                new MealSlot(FoodRole.Carb, 0.45m),
                new MealSlot(FoodRole.Protein, 0.40m),
                new MealSlot(FoodRole.Dairy, 0.15m, Required: false)
            ],
            [MealType.Lunch] =
            [
                new MealSlot(FoodRole.Protein, 0.40m),
                new MealSlot(FoodRole.Carb, 0.35m),
                new MealSlot(FoodRole.Vegetable, 0.25m)
            ],
            [MealType.Dinner] =
            [
                new MealSlot(FoodRole.Protein, 0.45m),
                new MealSlot(FoodRole.Carb, 0.30m),
                new MealSlot(FoodRole.Vegetable, 0.25m)
            ],
            [MealType.Snack] =
            [
                new MealSlot(FoodRole.Fruit, 0.55m),
                new MealSlot(FoodRole.Dairy, 0.30m, Required: false),
                new MealSlot(FoodRole.Condiment, 0.15m, Required: false)
            ]
        };

    public static FoodRole GetRole(FoodItem food) =>
        Roles.TryGetValue(food.Id, out var role) ? role : FoodRole.Carb;

    public static IReadOnlyList<MealSlot> GetSlots(MealType mealType) => MealTemplates[mealType];

    public static IReadOnlyList<FoodItem> FoodsByRole(
        IReadOnlyList<FoodItem> foods,
        FoodRole role,
        MealType mealType)
    {
        var list = foods.Where(f => GetRole(f) == role).OrderBy(f => f.Name).ToList();
        return mealType switch
        {
            MealType.Snack when role == FoodRole.Condiment =>
                list.Where(f => f.DefaultPortionGrams <= 90).ToList(),
            MealType.Lunch or MealType.Dinner when role == FoodRole.Condiment =>
                list.Where(f => f.Name.Contains("масло", StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => list
        };
    }

    public static FoodItem Pick(IReadOnlyList<FoodItem> candidates, int dayIndex, int slotIndex, Guid? excludeId = null)
    {
        var pool = candidates.Where(f => f.Id != excludeId).ToList();
        if (pool.Count == 0)
            pool = candidates.ToList();
        return pool[(dayIndex + slotIndex) % pool.Count];
    }

    public static IReadOnlyList<FoodItem> AlternativesFor(
        FoodItem primary,
        IReadOnlyList<FoodItem> foods,
        MealType mealType)
    {
        var role = GetRole(primary);
        if (role is FoodRole.Condiment or FoodRole.Vegetable)
            return [];

        return FoodsByRole(foods, role, mealType)
            .Where(f => f.Id != primary.Id)
            .Take(3)
            .ToList();
    }
}
