using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

public class WeekPlanRenewalService : IWeekPlanRenewalService
{
    private readonly ApplicationDbContext _db;
    private readonly IWeekPlanService _weekPlanService;
    private readonly ILogger<WeekPlanRenewalService> _logger;

    public WeekPlanRenewalService(
        ApplicationDbContext db,
        IWeekPlanService weekPlanService,
        ILogger<WeekPlanRenewalService> logger)
    {
        _db = db;
        _weekPlanService = weekPlanService;
        _logger = logger;
    }

    public async Task<int> RenewExpiredWeeksAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expiredBefore = today;

        var userIds = await _db.WeeklyPlans.AsNoTracking()
            .Where(p => p.IsActive && p.WeekStart.AddDays(7) <= expiredBefore)
            .Select(p => p.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var renewed = 0;
        foreach (var userId in userIds)
        {
            try
            {
                await _weekPlanService.RollWeekForwardAsync(userId, cancellationToken);
                renewed++;
                _logger.LogInformation("Автоматически создана новая неделя для пользователя {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось продлить неделю для пользователя {UserId}", userId);
            }
        }

        return renewed;
    }
}
