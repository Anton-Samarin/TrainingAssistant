using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Application.DTOs.Profile;

/// <summary>
/// Профиль пользователя для API
/// </summary>
public class ProfileDto
{
    public string Name { get; set; } = string.Empty;
    public UserSex Sex { get; set; }
    public int Age { get; set; }
    public decimal WeightKg { get; set; }
    public int HeightCm { get; set; }
    public UserGoal Goal { get; set; }
    public TrainingFocus TrainingFocus { get; set; }
    public FitnessLevel FitnessLevel { get; set; }
    public int SessionsPerWeek { get; set; }
    public int SessionDurationMin { get; set; }
    public int ActivityLevel { get; set; }
    public List<string> Equipment { get; set; } = new();
    public List<string> Injuries { get; set; } = new();
}
