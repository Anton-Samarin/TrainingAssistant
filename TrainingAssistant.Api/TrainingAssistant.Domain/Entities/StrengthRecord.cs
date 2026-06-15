namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Силовой показатель (упражнение, вес, повторения)
/// </summary>
public class StrengthRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly RecordDate { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
