namespace TrainingAssistant.Application.DTOs.Plans;

/// <summary>
/// Недельный план питания и тренировок
/// </summary>
public class WeekPlanDto
{
    public Guid Id { get; set; }
    public DateOnly WeekStart { get; set; }
    public string? ProgramType { get; set; }
    public double? ProgramConfidence { get; set; }
    public List<NutritionDayDto> NutritionDays { get; set; } = new();
    public List<TrainingDayDto> TrainingDays { get; set; } = new();
    public PlanHealthDto? Health { get; set; }
    public int CompletedExercises { get; set; }
    public int TotalExercises { get; set; }
    public int CompletedMealItems { get; set; }
    public int TotalMealItems { get; set; }
}

/// <summary>
/// Питание на один день недели
/// </summary>
public class NutritionDayDto
{
    public int DayIndex { get; set; }
    public int TargetCalories { get; set; }
    public int TargetProteinG { get; set; }
    public int TargetFatG { get; set; }
    public int TargetCarbsG { get; set; }
    public List<MealDto> Meals { get; set; } = new();
}

/// <summary>
/// Один приём пищи внутри дня
/// </summary>
public class MealDto
{
    public string MealType { get; set; } = string.Empty;
    public List<MealItemDto> Items { get; set; } = new();
}

/// <summary>
/// Блюдо с порцией в приёме пищи
/// </summary>
public class MealItemDto
{
    public Guid Id { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public int Grams { get; set; }
    public int Calories { get; set; }
    public bool IsAlternative { get; set; }
    public bool IsCompleted { get; set; }
}

/// <summary>
/// Тренировочный день в недельном плане
/// </summary>
public class TrainingDayDto
{
    public Guid Id { get; set; }
    public int DayIndex { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool IsRestDay { get; set; }
    public string? Focus { get; set; }
    public List<TrainingExerciseDto> Exercises { get; set; } = new();
}

/// <summary>
/// Упражнение в тренировочном дне
/// </summary>
public class TrainingExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public string Reps { get; set; } = string.Empty;
    public int RestSec { get; set; }
    public string? Equipment { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
}
