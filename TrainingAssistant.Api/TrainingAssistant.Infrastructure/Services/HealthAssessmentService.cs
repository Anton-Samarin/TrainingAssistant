using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// BMI, предупреждения и рекомендации по ВОЗ
/// </summary>
public class HealthAssessmentService : IHealthAssessmentService
{
    public double CalculateBmi(decimal weightKg, int heightCm)
    {
        var h = heightCm / 100.0;
        return Math.Round((double)weightKg / (h * h), 1);
    }

    public string GetBmiCategory(double bmi) => bmi switch
    {
        < 18.5 => "Недостаточная масса",
        < 25 => "Норма",
        < 30 => "Избыточная масса",
        < 35 => "Ожирение I степени",
        < 40 => "Ожирение II степени",
        _ => "Ожирение III степени"
    };

    public IReadOnlyList<string> GetWarnings(UserProfile profile, double bmi)
    {
        var list = new List<string>
        {
            "План носит рекомендательный характер и не заменяет консультацию врача."
        };

        if (bmi >= 30)
        {
            list.Add("ИМТ указывает на ожирение: перед интенсивными нагрузками рекомендуется консультация врача.");
            list.Add("Система подберёт щадящую нагрузку с акцентом на безопасные упражнения.");
        }
        else if (bmi >= 25)
            list.Add("ИМТ выше нормы: учтите умеренный дефицит калорий и постепенное увеличение активности.");

        if (bmi < 18.5)
            list.Add("ИМТ ниже нормы: агрессивный дефицит калорий не применяется.");

        if (profile.Age >= 50 && bmi >= 30)
            list.Add("Сочетание возраста 50+ и повышенного ИМТ: избегайте резких скачков нагрузки.");

        return list;
    }

    public IReadOnlyList<string> GetRecommendations(UserProfile profile, double bmi)
    {
        var list = new List<string>();

        if (bmi >= 35)
        {
            list.Add("Приоритет: низкоударные упражнения, короткие сессии, контроль пульса.");
            list.Add("Питание: умеренный дефицит калорий, достаточный белок для сохранения мышц.");
        }
        else if (bmi >= 30)
        {
            list.Add("Тренировки: 3–4 дня в неделю, кардио низкой интенсивности + силовые с собственным весом.");
            list.Add("Питание: дефицит около 10% от суточной нормы, не голодные диеты.");
        }
        else if (bmi >= 25 && profile.Goal == UserGoal.LoseWeight)
        {
            list.Add("Сочетайте силовые и кардио, следите за шагами и калориями.");
        }
        else if (bmi < 18.5 && profile.Goal == UserGoal.GainMuscle)
        {
            list.Add("Увеличьте калорийность и белок; прогрессия нагрузки — постепенная.");
        }
        else
            list.Add("Поддерживайте регулярность: план рассчитан на вашу цель и уровень подготовки.");

        if (profile.TrainingFocus == TrainingFocus.Endurance)
            list.Add("Фокус на выносливость: больше кардио и работы в пульсовых зонах низкой интенсивности.");
        else if (profile.TrainingFocus == TrainingFocus.Strength)
            list.Add("Фокус на силу: приоритет базовых движений и прогрессии веса/повторений.");

        return list;
    }
}
