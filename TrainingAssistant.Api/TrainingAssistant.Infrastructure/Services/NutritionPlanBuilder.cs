using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Сборка семидневного меню из каталога продуктов
/// </summary>
public class NutritionPlanBuilder
{
    private static readonly decimal[] MealCalorieShares = [0.25m, 0.35m, 0.30m, 0.10m];
    private static readonly MealType[] MealTypes =
        [MealType.Breakfast, MealType.Lunch, MealType.Dinner, MealType.Snack];

    /// <summary>
    /// Заполняет план питанием на семь дней из каталога блюд
    /// </summary>
    public void BuildWeekNutrition(
        WeeklyPlan plan,
        UserProfile profile,
        IReadOnlyList<FoodItem> foods,
        INutritionCalculator calculator)
    {
        var targets = calculator.CalculateDailyTargets(profile);

        for (var day = 1; day <= 7; day++)
        {
            var nutritionDay = new NutritionDay
            {
                Id = Guid.NewGuid(),
                WeeklyPlanId = plan.Id,
                DayIndex = day,
                TargetCalories = targets.Calories,
                TargetProteinG = targets.ProteinG,
                TargetFatG = targets.FatG,
                TargetCarbsG = targets.CarbsG
            };

            for (var i = 0; i < MealTypes.Length; i++)
            {
                var mealType = MealTypes[i];
                var mealTargetCalories = (int)Math.Round(targets.Calories * MealCalorieShares[i]);
                var meal = new Meal
                {
                    Id = Guid.NewGuid(),
                    NutritionDayId = nutritionDay.Id,
                    MealType = mealType,
                    SortOrder = i
                };

                BuildMeal(meal, mealType, mealTargetCalories, foods, day, i);
                nutritionDay.Meals.Add(meal);
            }

            plan.NutritionDays.Add(nutritionDay);
        }
    }

    private static void BuildMeal(
        Meal meal,
        MealType mealType,
        int mealTargetCalories,
        IReadOnlyList<FoodItem> foods,
        int dayIndex,
        int mealIndex)
    {
        var slots = FoodCatalog.GetSlots(mealType);
        var usedRoles = new HashSet<FoodRole>();
        var remaining = mealTargetCalories;

        for (var slotIndex = 0; slotIndex < slots.Count; slotIndex++)
        {
            var slot = slots[slotIndex];
            if (!slot.Required && remaining < 120)
                continue;

            var candidates = FoodCatalog.FoodsByRole(foods, slot.Role, mealType);
            if (candidates.Count == 0)
                continue;

            if (slot.Role == FoodRole.Condiment && mealType is MealType.Lunch or MealType.Dinner)
                continue;

            var food = FoodCatalog.Pick(candidates, dayIndex, mealIndex * 3 + slotIndex);

            var slotCalories = (int)Math.Round(mealTargetCalories * (double)slot.CalorieShare);
            slotCalories = Math.Min(slotCalories, remaining);

            if (!TryAddPortion(meal, food, slotCalories, isAlternative: false))
                continue;

            remaining -= meal.Items.Last().Calories;
            usedRoles.Add(slot.Role);

            foreach (var alt in FoodCatalog.AlternativesFor(food, foods, mealType))
            {
                if (remaining < 80)
                    break;
                TryAddPortion(meal, alt, slotCalories, isAlternative: true);
            }
        }

        // Лёгкая заправка только к полноценному обеду/ужину с белком и гарниром
        if (mealType is MealType.Lunch or MealType.Dinner
            && meal.Items.Count(i => !i.IsAlternative) >= 2
            && usedRoles.Contains(FoodRole.Protein)
            && remaining >= 40)
        {
            var oil = foods.FirstOrDefault(f => FoodCatalog.GetRole(f) == FoodRole.Condiment
                                                && f.Name.Contains("масло", StringComparison.OrdinalIgnoreCase));
            if (oil != null)
                TryAddPortion(meal, oil, Math.Min(45, remaining), isAlternative: false, maxGrams: 12);
        }
    }

    private static bool TryAddPortion(
        Meal meal,
        FoodItem food,
        int targetCalories,
        bool isAlternative,
        int? maxGrams = null)
    {
        if (targetCalories < 30)
            return false;

        var cap = maxGrams ?? 350;
        var maxByCalories = targetCalories * 100 / Math.Max(1, (int)food.CaloriesPer100g);
        var maxAllowed = Math.Min(cap, maxByCalories);
        if (maxAllowed < (FoodCatalog.GetRole(food) == FoodRole.Condiment ? 5 : 40))
            return false;

        var minGrams = FoodCatalog.GetRole(food) switch
        {
            FoodRole.Condiment => 5,
            FoodRole.Fruit => 60,
            _ => Math.Min(80, maxAllowed)
        };

        var grams = Math.Clamp(food.DefaultPortionGrams, minGrams, maxAllowed);
        var calories = (int)Math.Round(food.CaloriesPer100g * grams / 100m);

        if (calories <= 0)
            return false;

        if (meal.Items.Any(i => i.FoodItemId == food.Id && i.IsAlternative == isAlternative))
            return false;

        meal.Items.Add(new MealItem
        {
            Id = Guid.NewGuid(),
            MealId = meal.Id,
            FoodItemId = food.Id,
            Grams = grams,
            Calories = calories,
            IsAlternative = isAlternative
        });
        return true;
    }
}
