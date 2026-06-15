using TrainingAssistant.Application.DTOs.Plans;

namespace TrainingAssistant.Application.DTOs.Journal;

public class WeekSummaryDto
{
    public Guid Id { get; set; }
    public DateOnly WeekStart { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CompletedExercises { get; set; }
    public int TotalExercises { get; set; }
    public int CompletedMealItems { get; set; }
    public int TotalMealItems { get; set; }
}

public class DayJournalDto
{
    public DateOnly Date { get; set; }
    public Guid WeeklyPlanId { get; set; }
    public DateOnly WeekStart { get; set; }
    public int DayIndex { get; set; }
    public string DayName { get; set; } = string.Empty;
    public NutritionDayDto? Nutrition { get; set; }
    public TrainingDayDto? Training { get; set; }
}
