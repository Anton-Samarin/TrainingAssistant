using TrainingAssistant.Application.DTOs.Plans;

namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Формирование и выдача недельных планов
/// </summary>
public interface IWeekPlanService
{
    Task<WeekPlanDto> GenerateWeekAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Следующий 7-дневный период (для фонового продления)
    /// </summary>
    Task<WeekPlanDto> RollWeekForwardAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<WeekPlanDto?> GetCurrentWeekAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<WeekPlanDto> ReplaceExerciseAsync(
        Guid userId,
        Guid exerciseId,
        ReplaceExerciseRequest request,
        CancellationToken cancellationToken = default);

    Task<WeekPlanDto> AdjustTrainingDayAsync(
        Guid userId,
        Guid trainingDayId,
        AdjustTrainingDayRequest request,
        CancellationToken cancellationToken = default);

    Task<WeekPlanDto> SetExerciseCompletedAsync(
        Guid userId,
        Guid exerciseId,
        SetCompletedRequest request,
        CancellationToken cancellationToken = default);

    Task<WeekPlanDto> SetMealItemCompletedAsync(
        Guid userId,
        Guid mealItemId,
        SetCompletedRequest request,
        CancellationToken cancellationToken = default);

    IReadOnlyList<string> GetExerciseAlternatives(string exerciseName);
}
