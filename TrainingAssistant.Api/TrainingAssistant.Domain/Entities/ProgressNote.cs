namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Текстовая заметка о прогрессе (например, раз в неделю)
/// </summary>
public class ProgressNote
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly NoteDate { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
