using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Расчёт суточных калорий и БЖУ по формулам TDEE
/// </summary>
public class NutritionCalculator : INutritionCalculator
{
    /// <inheritdoc />
    public MacroTargets CalculateDailyTargets(UserProfile profile)
    {
        var bmr = CalculateBmr(profile);
        var activityMultiplier = profile.ActivityLevel switch
        {
            1 => 1.2m,
            2 => 1.375m,
            3 => 1.55m,
            4 => 1.725m,
            _ => 1.9m
        };

        var tdee = (int)Math.Round(bmr * activityMultiplier);
        var bmi = (double)profile.WeightKg / Math.Pow(profile.HeightCm / 100.0, 2);
        var loseMultiplier = bmi switch
        {
            >= 35 => 0.92m,
            >= 30 => 0.90m,
            _ => 0.85m
        };

        var calories = profile.Goal switch
        {
            UserGoal.LoseWeight => (int)Math.Round(tdee * loseMultiplier),
            UserGoal.GainMuscle => (int)Math.Round(tdee * 1.12m),
            _ => tdee
        };

        var proteinG = profile.Goal == UserGoal.GainMuscle
            ? (int)Math.Round((double)profile.WeightKg * 2.0)
            : (int)Math.Round((double)profile.WeightKg * 1.8);

        var fatG = (int)Math.Round(calories * 0.25 / 9);
        var carbsG = Math.Max(0, (int)Math.Round((calories - proteinG * 4 - fatG * 9) / 4.0));

        return new MacroTargets(calories, proteinG, fatG, carbsG);
    }

    private static decimal CalculateBmr(UserProfile profile)
    {
        var weight = (double)profile.WeightKg;
        return profile.Sex == UserSex.Male
            ? (decimal)(10 * weight + 6.25 * profile.HeightCm - 5 * profile.Age + 5)
            : (decimal)(10 * weight + 6.25 * profile.HeightCm - 5 * profile.Age - 161);
    }
}
