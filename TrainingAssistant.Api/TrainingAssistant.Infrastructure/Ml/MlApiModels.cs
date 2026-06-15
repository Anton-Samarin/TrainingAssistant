using System.Text.Json.Serialization;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Domain.Enums;

namespace TrainingAssistant.Infrastructure.Ml;

/// <summary>
/// Тело запроса к эндпоинту генерации недели ML
/// </summary>
public class MlGenerateWeekRequest
{
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    [JsonPropertyName("goal")]
    public string Goal { get; set; } = string.Empty;

    [JsonPropertyName("sex")]
    public string Sex { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("weight_kg")]
    public double WeightKg { get; set; }

    [JsonPropertyName("height_cm")]
    public int HeightCm { get; set; }

    [JsonPropertyName("fitness_level")]
    public string FitnessLevel { get; set; } = string.Empty;

    [JsonPropertyName("sessions_per_week")]
    public int SessionsPerWeek { get; set; }

    [JsonPropertyName("session_duration_min")]
    public int SessionDurationMin { get; set; }

    [JsonPropertyName("activity_level")]
    public int ActivityLevel { get; set; }

    [JsonPropertyName("equipment")]
    public List<string> Equipment { get; set; } = new();

    [JsonPropertyName("injuries")]
    public List<string> Injuries { get; set; } = new();

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = "ru";

    [JsonPropertyName("training_focus")]
    public string TrainingFocus { get; set; } = "mixed";

    [JsonPropertyName("exercise_preferences")]
    public List<MlExercisePreference> ExercisePreferences { get; set; } = new();

    /// <summary>
    /// Собирает тело запроса к ML из профиля пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="profile">Профиль с целями и антропометрией</param>
    /// <param name="equipment">Список доступного оборудования</param>
    /// <param name="injuries">Список ограничений по здоровью</param>
    /// <returns>Модель запроса для ML API</returns>
    public static MlGenerateWeekRequest FromProfile(
        Guid userId,
        Domain.Entities.UserProfile profile,
        List<string> equipment,
        List<string> injuries,
        List<UserExercisePreference>? preferences = null)
    {
        return new MlGenerateWeekRequest
        {
            UserId = userId,
            Goal = MapGoal(profile.Goal),
            Sex = profile.Sex == UserSex.Male ? "male" : "female",
            Age = profile.Age,
            WeightKg = (double)profile.WeightKg,
            HeightCm = profile.HeightCm,
            FitnessLevel = MapLevel(profile.FitnessLevel),
            SessionsPerWeek = profile.SessionsPerWeek,
            SessionDurationMin = profile.SessionDurationMin,
            ActivityLevel = profile.ActivityLevel,
            Equipment = equipment,
            Injuries = injuries,
            Locale = "ru",
            TrainingFocus = MapTrainingFocus(profile.TrainingFocus),
            ExercisePreferences = (preferences ?? [])
                .Select(p => new MlExercisePreference
                {
                    AvoidName = p.AvoidExerciseName,
                    PreferredName = p.PreferredExerciseName,
                    PoolKey = p.PoolKey
                }).ToList()
        };
    }

    private static string MapTrainingFocus(Domain.Enums.TrainingFocus focus) => focus switch
    {
        Domain.Enums.TrainingFocus.Strength => "strength",
        Domain.Enums.TrainingFocus.Endurance => "endurance",
        _ => "mixed"
    };

    private static string MapGoal(UserGoal goal) => goal switch
    {
        UserGoal.LoseWeight => "lose_weight",
        UserGoal.GainMuscle => "gain_muscle",
        _ => "maintain"
    };

    private static string MapLevel(Domain.Enums.FitnessLevel level) => level switch
    {
        Domain.Enums.FitnessLevel.Intermediate => "intermediate",
        Domain.Enums.FitnessLevel.Advanced => "advanced",
        _ => "beginner"
    };
}

/// <summary>
/// Ответ ML с типом программы и неделей тренировок
/// </summary>
public class MlGenerateWeekResponse
{
    [JsonPropertyName("program_type")]
    public string ProgramType { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("week")]
    public MlWeek Week { get; set; } = new();
}

/// <summary>
/// Неделя тренировок в ответе ML
/// </summary>
public class MlWeek
{
    [JsonPropertyName("days")]
    public List<MlTrainingDay> Days { get; set; } = new();
}

/// <summary>
/// Один день тренировок в JSON ML
/// </summary>
public class MlTrainingDay
{
    [JsonPropertyName("day_index")]
    public int DayIndex { get; set; }

    [JsonPropertyName("day_name")]
    public string DayName { get; set; } = string.Empty;

    [JsonPropertyName("is_rest_day")]
    public bool IsRestDay { get; set; }

    [JsonPropertyName("focus")]
    public string? Focus { get; set; }

    [JsonPropertyName("exercises")]
    public List<MlExercise> Exercises { get; set; } = new();
}

/// <summary>
/// Упражнение в ответе ML
/// </summary>
public class MlExercise
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sets")]
    public int Sets { get; set; }

    [JsonPropertyName("reps")]
    public string Reps { get; set; } = string.Empty;

    [JsonPropertyName("rest_sec")]
    public int RestSec { get; set; }

    [JsonPropertyName("equipment")]
    public string? Equipment { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
