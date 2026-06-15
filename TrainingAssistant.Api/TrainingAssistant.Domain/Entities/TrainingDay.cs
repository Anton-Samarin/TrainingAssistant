namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Тренировочный или выходной день в плане
/// </summary>
public class TrainingDay
{
    public Guid Id { get; set; }
    public Guid WeeklyPlanId { get; set; }
    public int DayIndex { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool IsRestDay { get; set; }
    public string? Focus { get; set; }

    public WeeklyPlan WeeklyPlan { get; set; } = null!;
    public ICollection<TrainingExercise> Exercises { get; set; } = new List<TrainingExercise>();
}
