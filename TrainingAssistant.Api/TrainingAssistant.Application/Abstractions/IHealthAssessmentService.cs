using TrainingAssistant.Domain.Entities;

namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Оценка BMI, предупреждений и текстовых рекомендаций
/// </summary>
public interface IHealthAssessmentService
{
    double CalculateBmi(decimal weightKg, int heightCm);

    string GetBmiCategory(double bmi);

    IReadOnlyList<string> GetWarnings(UserProfile profile, double bmi);

    IReadOnlyList<string> GetRecommendations(UserProfile profile, double bmi);
}
