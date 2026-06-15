using TrainingAssistant.Application.DTOs.Plans;
using TrainingAssistant.Domain.Entities;

namespace TrainingAssistant.Infrastructure.Mapping;

/// <summary>
/// Маппинг сущностей недельного плана в DTO
/// </summary>
public static class WeekPlanMapper
{
    public static WeekPlanDto ToDto(WeeklyPlan plan, PlanHealthDto? health = null)
    {
        var exercises = plan.TrainingDays.SelectMany(d => d.Exercises).ToList();
        var mealItems = plan.NutritionDays.SelectMany(d => d.Meals).SelectMany(m => m.Items).ToList();

        return new WeekPlanDto
        {
            Id = plan.Id,
            WeekStart = plan.WeekStart,
            ProgramType = plan.ProgramType,
            ProgramConfidence = plan.ProgramConfidence,
            Health = health,
            CompletedExercises = exercises.Count(e => e.IsCompleted),
            TotalExercises = exercises.Count,
            CompletedMealItems = mealItems.Count(i => i.IsCompleted),
            TotalMealItems = mealItems.Count,
            NutritionDays = plan.NutritionDays
                .OrderBy(x => x.DayIndex)
                .Select(day => new NutritionDayDto
                {
                    DayIndex = day.DayIndex,
                    TargetCalories = day.TargetCalories,
                    TargetProteinG = day.TargetProteinG,
                    TargetFatG = day.TargetFatG,
                    TargetCarbsG = day.TargetCarbsG,
                    Meals = day.Meals
                        .OrderBy(m => m.SortOrder)
                        .Select(m => new MealDto
                        {
                            MealType = m.MealType.ToString(),
                            Items = m.Items
                                .OrderBy(i => i.IsAlternative)
                                .ThenBy(i => i.FoodItem?.Name)
                                .Select(i => new MealItemDto
                                {
                                    Id = i.Id,
                                    FoodName = i.FoodItem?.Name ?? string.Empty,
                                    Grams = i.Grams,
                                    Calories = i.Calories,
                                    IsAlternative = i.IsAlternative,
                                    IsCompleted = i.IsCompleted
                                }).ToList()
                        }).ToList()
                }).ToList(),
            TrainingDays = plan.TrainingDays
                .OrderBy(x => x.DayIndex)
                .Select(day => new TrainingDayDto
                {
                    Id = day.Id,
                    DayIndex = day.DayIndex,
                    DayName = day.DayName,
                    IsRestDay = day.IsRestDay,
                    Focus = day.Focus,
                    Exercises = day.Exercises
                        .OrderBy(e => e.SortOrder)
                        .Select(e => new TrainingExerciseDto
                        {
                            Id = e.Id,
                            Name = e.Name,
                            Sets = e.Sets,
                            Reps = e.Reps,
                            RestSec = e.RestSec,
                            Equipment = e.Equipment,
                            Notes = e.Notes,
                            IsCompleted = e.IsCompleted
                        }).ToList()
                }).ToList()
        };
    }
}
