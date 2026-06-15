using System.ComponentModel.DataAnnotations;
using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Application.DTOs.Auth;

/// <summary>
/// Данные для регистрации и первичного профиля
/// </summary>
public class RegisterRequest
{
    [Required, MinLength(2), MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public UserSex Sex { get; set; }
    [Range(14, 80)]
    public int Age { get; set; }
    [Range(30, 250)]
    public decimal WeightKg { get; set; }
    [Range(130, 220)]
    public int HeightCm { get; set; }
    public UserGoal Goal { get; set; }
    public TrainingFocus TrainingFocus { get; set; } = TrainingFocus.Mixed;
    public FitnessLevel FitnessLevel { get; set; }
    [Range(2, 6)]
    public int SessionsPerWeek { get; set; } = 3;
    [Range(20, 120)]
    public int SessionDurationMin { get; set; } = 45;
    [Range(1, 5)]
    public int ActivityLevel { get; set; } = 3;
    public List<string> Equipment { get; set; } = new();
    public List<string> Injuries { get; set; } = new();
}
