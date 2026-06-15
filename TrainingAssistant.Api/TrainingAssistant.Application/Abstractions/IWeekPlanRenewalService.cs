namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Автоматическое создание новой недели по истечении периода
/// </summary>
public interface IWeekPlanRenewalService
{
    Task<int> RenewExpiredWeeksAsync(CancellationToken cancellationToken = default);
}
