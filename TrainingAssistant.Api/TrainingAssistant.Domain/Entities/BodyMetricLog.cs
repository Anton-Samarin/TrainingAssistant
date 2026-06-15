namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Запись веса и заметки на дату
/// </summary>
public class BodyMetricLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly LogDate { get; set; }
    public decimal WeightKg { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
