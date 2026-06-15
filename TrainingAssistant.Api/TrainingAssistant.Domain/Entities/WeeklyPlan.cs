namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Сгенерированный недельный план пользователя
/// </summary>
public class WeeklyPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly WeekStart { get; set; }
    public bool IsActive { get; set; }
    public string? ProgramType { get; set; }
    public double? ProgramConfidence { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public ICollection<NutritionDay> NutritionDays { get; set; } = new List<NutritionDay>();
    public ICollection<TrainingDay> TrainingDays { get; set; } = new List<TrainingDay>();
}
