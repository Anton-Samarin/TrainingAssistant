namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Упражнение в тренировочном дне
/// </summary>
public class TrainingExercise
{
    public Guid Id { get; set; }
    public Guid TrainingDayId { get; set; }
    public int SortOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public string Reps { get; set; } = string.Empty;
    public int RestSec { get; set; }
    public string? Equipment { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }

    public TrainingDay TrainingDay { get; set; } = null!;
}
