using Microsoft.EntityFrameworkCore;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Plans;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Infrastructure.Helpers;
using TrainingAssistant.Infrastructure.Mapping;
using TrainingAssistant.Infrastructure.Ml;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Генерация недельного плана, правки упражнений и трекинг
/// </summary>
public class WeekPlanService : IWeekPlanService
{
    private readonly ApplicationDbContext _db;
    private readonly INutritionCalculator _nutritionCalculator;
    private readonly NutritionPlanBuilder _nutritionPlanBuilder;
    private readonly MlTrainingClient _mlClient;
    private readonly IHealthAssessmentService _health;

    public WeekPlanService(
        ApplicationDbContext db,
        INutritionCalculator nutritionCalculator,
        NutritionPlanBuilder nutritionPlanBuilder,
        MlTrainingClient mlClient,
        IHealthAssessmentService health)
    {
        _db = db;
        _nutritionCalculator = nutritionCalculator;
        _nutritionPlanBuilder = nutritionPlanBuilder;
        _mlClient = mlClient;
        _health = health;
    }

    public async Task<WeekPlanDto> GenerateWeekAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Пользователь не найден.");

        var active = await _db.WeeklyPlans.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive, cancellationToken);

        var weekStart = active?.WeekStart
            ?? WeekPeriodHelper.GetCurrentPeriodStart(user.CreatedAtUtc, DateTime.UtcNow);

        return await CreateWeekPlanAsync(userId, weekStart, cancellationToken);
    }

    public async Task<WeekPlanDto> RollWeekForwardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var active = await _db.WeeklyPlans
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("Активный план не найден.");

        if (!WeekPeriodHelper.IsPeriodExpired(active.WeekStart, DateTime.UtcNow))
            throw new InvalidOperationException("Текущий период ещё не завершён.");

        return await CreateWeekPlanAsync(userId, active.WeekStart.AddDays(7), cancellationToken);
    }

    private async Task<WeekPlanDto> CreateWeekPlanAsync(
        Guid userId,
        DateOnly weekStart,
        CancellationToken cancellationToken)
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Профиль не найден.");

        var foods = await _db.FoodItems.AsNoTracking().ToListAsync(cancellationToken);
        if (foods.Count == 0)
            throw new InvalidOperationException("Каталог блюд пуст. Перезапустите API для инициализации БД.");

        var preferences = await _db.UserExercisePreferences.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var activePlans = await _db.WeeklyPlans.Where(x => x.UserId == userId && x.IsActive).ToListAsync(cancellationToken);
        foreach (var p in activePlans)
            p.IsActive = false;

        var plan = new WeeklyPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WeekStart = weekStart,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _nutritionPlanBuilder.BuildWeekNutrition(plan, profile, foods, _nutritionCalculator);

        var mlResponse = await _mlClient.GenerateWeekAsync(userId, profile, preferences, cancellationToken);
        plan.ProgramType = mlResponse.ProgramType;
        plan.ProgramConfidence = mlResponse.Confidence;

        foreach (var day in mlResponse.Week.Days.OrderBy(x => x.DayIndex))
        {
            var trainingDay = new TrainingDay
            {
                Id = Guid.NewGuid(),
                WeeklyPlanId = plan.Id,
                DayIndex = day.DayIndex,
                DayName = day.DayName,
                IsRestDay = day.IsRestDay,
                Focus = day.Focus
            };

            var order = 0;
            foreach (var ex in day.Exercises)
            {
                trainingDay.Exercises.Add(new TrainingExercise
                {
                    Id = Guid.NewGuid(),
                    TrainingDayId = trainingDay.Id,
                    SortOrder = order++,
                    Name = ex.Name,
                    Sets = ex.Sets,
                    Reps = ex.Reps,
                    RestSec = ex.RestSec,
                    Equipment = ex.Equipment,
                    Notes = ex.Notes
                });
            }

            plan.TrainingDays.Add(trainingDay);
        }

        _db.WeeklyPlans.Add(plan);
        await _db.SaveChangesAsync(cancellationToken);

        return await LoadWeekDtoAsync(plan.Id, profile, cancellationToken)
            ?? throw new InvalidOperationException("Не удалось загрузить созданный план.");
    }

    public Task<WeekPlanDto?> GetCurrentWeekAsync(Guid userId, CancellationToken cancellationToken = default)
        => LoadActiveWeekAsync(userId, cancellationToken);

    public async Task<WeekPlanDto> ReplaceExerciseAsync(
        Guid userId,
        Guid exerciseId,
        ReplaceExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var exercise = await _db.TrainingExercises
            .Include(x => x.TrainingDay).ThenInclude(x => x.WeeklyPlan)
            .FirstOrDefaultAsync(x => x.Id == exerciseId, cancellationToken)
            ?? throw new KeyNotFoundException("Упражнение не найдено.");

        if (exercise.TrainingDay.WeeklyPlan.UserId != userId || !exercise.TrainingDay.WeeklyPlan.IsActive)
            throw new KeyNotFoundException("Упражнение не найдено.");

        var oldName = exercise.Name;
        exercise.Name = request.NewName.Trim();

        if (request.RememberForFuture)
        {
            var pool = ExerciseCatalog.GuessPoolForExercise(oldName);
            var existing = await _db.UserExercisePreferences
                .FirstOrDefaultAsync(
                    x => x.UserId == userId && x.AvoidExerciseName == oldName,
                    cancellationToken);

            if (existing is null)
            {
                _db.UserExercisePreferences.Add(new UserExercisePreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    AvoidExerciseName = oldName,
                    PreferredExerciseName = exercise.Name,
                    PoolKey = pool,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }
            else
            {
                existing.PreferredExerciseName = exercise.Name;
                existing.PoolKey = pool;
                existing.CreatedAtUtc = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return await LoadWeekDtoForPlanAsync(exercise.TrainingDay.WeeklyPlanId, userId, cancellationToken);
    }

    public async Task<WeekPlanDto> AdjustTrainingDayAsync(
        Guid userId,
        Guid trainingDayId,
        AdjustTrainingDayRequest request,
        CancellationToken cancellationToken = default)
    {
        var day = await _db.TrainingDays
            .Include(x => x.Exercises)
            .Include(x => x.WeeklyPlan)
            .FirstOrDefaultAsync(x => x.Id == trainingDayId, cancellationToken)
            ?? throw new KeyNotFoundException("Тренировочный день не найден.");

        if (day.WeeklyPlan.UserId != userId || !day.WeeklyPlan.IsActive || day.IsRestDay)
            throw new InvalidOperationException("Нельзя изменить этот день.");

        var easier = string.Equals(request.Mode, "easier", StringComparison.OrdinalIgnoreCase);
        foreach (var ex in day.Exercises)
        {
            if (easier)
            {
                ex.Sets = Math.Max(2, ex.Sets - 1);
                ex.RestSec = Math.Max(45, ex.RestSec - 15);
                if (ex.Name.Contains("прыжк", StringComparison.OrdinalIgnoreCase)
                    || ex.Name.Contains("Берпи", StringComparison.OrdinalIgnoreCase))
                    ex.Name = "Шаги на месте";
            }
            else
            {
                ex.Sets = Math.Min(4, ex.Sets + 1);
                ex.RestSec = Math.Min(120, ex.RestSec + 15);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return await LoadWeekDtoForPlanAsync(day.WeeklyPlanId, userId, cancellationToken);
    }

    public async Task<WeekPlanDto> SetExerciseCompletedAsync(
        Guid userId,
        Guid exerciseId,
        SetCompletedRequest request,
        CancellationToken cancellationToken = default)
    {
        var exercise = await LoadOwnedExerciseAsync(userId, exerciseId, cancellationToken);
        exercise.IsCompleted = request.IsCompleted;
        await _db.SaveChangesAsync(cancellationToken);
        return await LoadWeekDtoForPlanAsync(exercise.TrainingDay.WeeklyPlanId, userId, cancellationToken);
    }

    public async Task<WeekPlanDto> SetMealItemCompletedAsync(
        Guid userId,
        Guid mealItemId,
        SetCompletedRequest request,
        CancellationToken cancellationToken = default)
    {
        var item = await _db.MealItems
            .Include(x => x.Meal).ThenInclude(x => x.NutritionDay).ThenInclude(x => x.WeeklyPlan)
            .FirstOrDefaultAsync(x => x.Id == mealItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Позиция питания не найдена.");

        if (item.Meal.NutritionDay.WeeklyPlan.UserId != userId || !item.Meal.NutritionDay.WeeklyPlan.IsActive)
            throw new KeyNotFoundException("Позиция питания не найдена.");

        item.IsCompleted = request.IsCompleted;
        await _db.SaveChangesAsync(cancellationToken);
        return await LoadWeekDtoForPlanAsync(item.Meal.NutritionDay.WeeklyPlanId, userId, cancellationToken);
    }

    public IReadOnlyList<string> GetExerciseAlternatives(string exerciseName)
    {
        var pool = ExerciseCatalog.GuessPoolForExercise(exerciseName);
        return ExerciseCatalog.GetAlternatives(pool, exerciseName);
    }

    private async Task<TrainingExercise> LoadOwnedExerciseAsync(
        Guid userId,
        Guid exerciseId,
        CancellationToken cancellationToken)
    {
        var exercise = await _db.TrainingExercises
            .Include(x => x.TrainingDay).ThenInclude(x => x.WeeklyPlan)
            .FirstOrDefaultAsync(x => x.Id == exerciseId, cancellationToken)
            ?? throw new KeyNotFoundException("Упражнение не найдено.");

        if (exercise.TrainingDay.WeeklyPlan.UserId != userId || !exercise.TrainingDay.WeeklyPlan.IsActive)
            throw new KeyNotFoundException("Упражнение не найдено.");

        return exercise;
    }

    private async Task<WeekPlanDto?> LoadActiveWeekAsync(Guid userId, CancellationToken cancellationToken)
    {
        var planId = await _db.WeeklyPlans.AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (planId == Guid.Empty)
            return null;

        var profile = await _db.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return profile is null
            ? null
            : await LoadWeekDtoAsync(planId, profile, cancellationToken);
    }

    private async Task<WeekPlanDto> LoadWeekDtoForPlanAsync(
        Guid planId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var profile = await _db.UserProfiles.AsNoTracking()
            .FirstAsync(x => x.UserId == userId, cancellationToken);
        return await LoadWeekDtoAsync(planId, profile, cancellationToken)
            ?? throw new InvalidOperationException("План не найден.");
    }

    private async Task<WeekPlanDto?> LoadWeekDtoAsync(
        Guid planId,
        Domain.Entities.UserProfile profile,
        CancellationToken cancellationToken)
    {
        var plan = await _db.WeeklyPlans
            .AsNoTracking()
            .Include(x => x.NutritionDays).ThenInclude(x => x.Meals).ThenInclude(x => x.Items).ThenInclude(x => x.FoodItem)
            .Include(x => x.TrainingDays).ThenInclude(x => x.Exercises)
            .FirstOrDefaultAsync(x => x.Id == planId, cancellationToken);

        if (plan is null)
            return null;

        var bmi = _health.CalculateBmi(profile.WeightKg, profile.HeightCm);
        var health = new PlanHealthDto
        {
            Bmi = bmi,
            BmiCategory = _health.GetBmiCategory(bmi),
            Warnings = _health.GetWarnings(profile, bmi).ToList(),
            Recommendations = _health.GetRecommendations(profile, bmi).ToList()
        };

        return WeekPlanMapper.ToDto(plan, health);
    }
}
