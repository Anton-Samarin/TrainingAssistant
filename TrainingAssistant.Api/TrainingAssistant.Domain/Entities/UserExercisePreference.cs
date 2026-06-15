namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Замена упражнения, выбранная пользователем — учитывается при следующих генерациях
/// </summary>
public class UserExercisePreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AvoidExerciseName { get; set; } = string.Empty;
    public string PreferredExerciseName { get; set; } = string.Empty;
    public string? PoolKey { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
