using TrainingAssistant.Application.DTOs.Journal;
using TrainingAssistant.Application.DTOs.Plans;

namespace TrainingAssistant.Application.Abstractions;

public interface IJournalService
{
    Task<IReadOnlyList<WeekSummaryDto>> ListWeeksAsync(Guid userId, int limit = 30, CancellationToken cancellationToken = default);

    Task<WeekPlanDto?> GetWeekAsync(Guid userId, Guid planId, CancellationToken cancellationToken = default);

    Task<DayJournalDto?> GetDayAsync(
        Guid userId,
        DateOnly date,
        Guid? planId = null,
        CancellationToken cancellationToken = default);
}
