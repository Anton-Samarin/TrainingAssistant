namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// День питания внутри недельного плана
/// </summary>
public class NutritionDay
{
    public Guid Id { get; set; }
    public Guid WeeklyPlanId { get; set; }
    public int DayIndex { get; set; }
    public int TargetCalories { get; set; }
    public int TargetProteinG { get; set; }
    public int TargetFatG { get; set; }
    public int TargetCarbsG { get; set; }

    public WeeklyPlan WeeklyPlan { get; set; } = null!;
    public ICollection<Meal> Meals { get; set; } = new List<Meal>();
}
