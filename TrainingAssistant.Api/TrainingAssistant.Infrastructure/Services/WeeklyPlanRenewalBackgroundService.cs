using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.Options;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Периодически создаёт новую неделю, когда 7-дневный период от даты регистрации истёк
/// </summary>
public class WeeklyPlanRenewalBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WeekRenewalOptions _options;
    private readonly ILogger<WeeklyPlanRenewalBackgroundService> _logger;

    public WeeklyPlanRenewalBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<WeekRenewalOptions> options,
        ILogger<WeeklyPlanRenewalBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(1, _options.IntervalMinutes));
        _logger.LogInformation(
            "Фоновое продление недель: {Status}, интервал {Minutes} мин.",
            _options.Enabled ? "включено" : "выключено",
            interval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_options.Enabled)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var renewal = scope.ServiceProvider.GetRequiredService<IWeekPlanRenewalService>();
                    var count = await renewal.RenewExpiredWeeksAsync(stoppingToken);
                    if (count > 0)
                        _logger.LogInformation("Продлено недель: {Count}", count);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Ошибка фонового продления недель");
                }
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
