namespace TrainingAssistant.Application.DTOs.Plans;

/// <summary>
/// BMI, предупреждения и рекомендации для недельного плана
/// </summary>
public class PlanHealthDto
{
    public double Bmi { get; set; }
    public string BmiCategory { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}
