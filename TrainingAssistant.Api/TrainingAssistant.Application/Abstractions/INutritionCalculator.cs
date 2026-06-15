using TrainingAssistant.Domain.Entities;

namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Целевые калории и БЖУ на день
/// </summary>
/// <param name="Calories">Калории</param>
/// <param name="ProteinG">Белки, г</param>
/// <param name="FatG">Жиры, г</param>
/// <param name="CarbsG">Углеводы, г</param>
public record MacroTargets(int Calories, int ProteinG, int FatG, int CarbsG);

/// <summary>
/// Расчёт нутриентов по профилю
/// </summary>
public interface INutritionCalculator
{
    /// <summary>
    /// Считает суточные калории и макросы под цель пользователя
    /// </summary>
    /// <param name="profile">Профиль с антропометрией и целью</param>
    /// <returns>Целевые значения на один день</returns>
    MacroTargets CalculateDailyTargets(UserProfile profile);
}
