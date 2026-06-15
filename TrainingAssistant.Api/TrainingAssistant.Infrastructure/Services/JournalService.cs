using Microsoft.EntityFrameworkCore;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Journal;
using TrainingAssistant.Application.DTOs.Plans;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Infrastructure.Mapping;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

public class JournalService : IJournalService
{
    private readonly ApplicationDbContext _db;
    private readonly IHealthAssessmentService _health;

    public JournalService(ApplicationDbContext db, IHealthAssessmentService health)
    {
        _db = db;
        _health = health;
    }

    public async Task<IReadOnlyList<WeekSummaryDto>> ListWeeksAsync(
        Guid userId,
        int limit = 30,
        CancellationToken cancellationToken = default)
    {
        var plans = await _db.WeeklyPlans.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(limit)
            .Include(x => x.TrainingDays).ThenInclude(x => x.Exercises)
            .Include(x => x.NutritionDays).ThenInclude(x => x.Meals).ThenInclude(x => x.Items)
            .ToListAsync(cancellationToken);

        return plans.Select(MapSummary).ToList();
    }

    public async Task<WeekPlanDto?> GetWeekAsync(Guid userId, Guid planId, CancellationToken cancellationToken = default)
    {
        var plan = await LoadPlanAsync(planId, userId, cancellationToken);
        if (plan is null)
            return null;

        var profile = await _db.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        PlanHealthDto? health = null;
        if (profile is not null)
        {
            var bmi = _health.CalculateBmi(profile.WeightKg, profile.HeightCm);
            health = new PlanHealthDto
            {
                Bmi = bmi,
                BmiCategory = _health.GetBmiCategory(bmi),
                Warnings = _health.GetWarnings(profile, bmi).ToList(),
                Recommendations = _health.GetRecommendations(profile, bmi).ToList()
            };
        }

        return WeekPlanMapper.ToDto(plan, health);
    }

    public async Task<DayJournalDto?> GetDayAsync(
        Guid userId,
        DateOnly date,
        Guid? planId = null,
        CancellationToken cancellationToken = default)
    {
        WeeklyPlan? plan;

        if (planId is not null)
        {
            plan = await _db.WeeklyPlans.AsNoTracking()
                .Where(x => x.Id == planId.Value && x.UserId == userId)
                .Include(x => x.TrainingDays).ThenInclude(x => x.Exercises)
                .Include(x => x.NutritionDays).ThenInclude(x => x.Meals).ThenInclude(x => x.Items).ThenInclude(x => x.FoodItem)
                .FirstOrDefaultAsync(cancellationToken);

            if (plan is null || date < plan.WeekStart || date > plan.WeekStart.AddDays(6))
                return null;
        }
        else
        {
            var weekStartMax = date;
            var weekStartMin = date.AddDays(-6);

            plan = await _db.WeeklyPlans.AsNoTracking()
                .Where(x => x.UserId == userId && x.WeekStart <= weekStartMax && x.WeekStart >= weekStartMin)
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Include(x => x.TrainingDays).ThenInclude(x => x.Exercises)
                .Include(x => x.NutritionDays).ThenInclude(x => x.Meals).ThenInclude(x => x.Items).ThenInclude(x => x.FoodItem)
                .FirstOrDefaultAsync(cancellationToken);

            if (plan is null)
                return null;
        }

        var dayIndex = date.DayNumber - plan.WeekStart.DayNumber + 1;
        if (dayIndex < 1 || dayIndex > 7)
            return null;

        var dto = WeekPlanMapper.ToDto(plan);
        var nutrition = dto.NutritionDays.FirstOrDefault(x => x.DayIndex == dayIndex);
        var training = dto.TrainingDays.FirstOrDefault(x => x.DayIndex == dayIndex);
        var trEntity = plan.TrainingDays.FirstOrDefault(x => x.DayIndex == dayIndex);
        var dayName = trEntity?.DayName ?? training?.DayName ?? $"День {dayIndex}";

        return new DayJournalDto
        {
            Date = date,
            WeeklyPlanId = plan.Id,
            WeekStart = plan.WeekStart,
            DayIndex = dayIndex,
            DayName = dayName,
            Nutrition = nutrition,
            Training = training
        };
    }

    private async Task<WeeklyPlan?> LoadPlanAsync(Guid planId, Guid userId, CancellationToken cancellationToken)
    {
        return await _db.WeeklyPlans.AsNoTracking()
            .Include(x => x.NutritionDays).ThenInclude(x => x.Meals).ThenInclude(x => x.Items).ThenInclude(x => x.FoodItem)
            .Include(x => x.TrainingDays).ThenInclude(x => x.Exercises)
            .FirstOrDefaultAsync(x => x.Id == planId && x.UserId == userId, cancellationToken);
    }

    private static WeekSummaryDto MapSummary(WeeklyPlan plan)
    {
        var exercises = plan.TrainingDays.SelectMany(d => d.Exercises).ToList();
        var mealItems = plan.NutritionDays.SelectMany(d => d.Meals).SelectMany(m => m.Items).ToList();

        return new WeekSummaryDto
        {
            Id = plan.Id,
            WeekStart = plan.WeekStart,
            IsActive = plan.IsActive,
            CreatedAtUtc = plan.CreatedAtUtc,
            CompletedExercises = exercises.Count(e => e.IsCompleted),
            TotalExercises = exercises.Count,
            CompletedMealItems = mealItems.Count(i => i.IsCompleted),
            TotalMealItems = mealItems.Count
        };
    }
}
