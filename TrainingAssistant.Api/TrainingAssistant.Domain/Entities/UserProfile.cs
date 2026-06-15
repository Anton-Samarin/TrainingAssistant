using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Анкета и цели пользователя для расчёта планов
/// </summary>
public class UserProfile
{
    public Guid UserId { get; set; }
    public UserSex Sex { get; set; }
    public int Age { get; set; }
    public decimal WeightKg { get; set; }
    public int HeightCm { get; set; }
    public UserGoal Goal { get; set; }
    public TrainingFocus TrainingFocus { get; set; } = TrainingFocus.Mixed;
    public FitnessLevel FitnessLevel { get; set; }
    public int SessionsPerWeek { get; set; }
    public int SessionDurationMin { get; set; }
    public int ActivityLevel { get; set; }
    public string EquipmentJson { get; set; } = "[]";
    public string InjuriesJson { get; set; } = "[]";
    public DateTime UpdatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
