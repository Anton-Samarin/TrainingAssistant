using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Приём пищи в дне питания
/// </summary>
public class Meal
{
    public Guid Id { get; set; }
    public Guid NutritionDayId { get; set; }
    public MealType MealType { get; set; }
    public int SortOrder { get; set; }

    public NutritionDay NutritionDay { get; set; } = null!;
    public ICollection<MealItem> Items { get; set; } = new List<MealItem>();
}
