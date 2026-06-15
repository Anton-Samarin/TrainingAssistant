namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Порция продукта в приёме пищи
/// </summary>
public class MealItem
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public Guid FoodItemId { get; set; }
    public int Grams { get; set; }
    public int Calories { get; set; }
    /// <summary>Вариант замены основного блюда в том же слоте</summary>
    public bool IsAlternative { get; set; }
    public bool IsCompleted { get; set; }

    public Meal Meal { get; set; } = null!;
    public FoodItem FoodItem { get; set; } = null!;
}
