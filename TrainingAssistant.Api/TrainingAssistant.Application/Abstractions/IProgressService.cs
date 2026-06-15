using TrainingAssistant.Application.DTOs.Progress;

namespace TrainingAssistant.Application.Abstractions;

public interface IProgressService
{
    Task<ProgressOverviewDto> GetOverviewAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<WeightProfileSyncDto> AddWeightLogAsync(Guid userId, CreateBodyMetricRequest request, CancellationToken cancellationToken = default);

    Task<WeightProfileSyncDto> DeleteWeightLogAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

    Task<StrengthRecordDto> AddStrengthRecordAsync(Guid userId, CreateStrengthRecordRequest request, CancellationToken cancellationToken = default);

    Task DeleteStrengthRecordAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

    Task<ProgressNoteDto> AddNoteAsync(Guid userId, CreateProgressNoteRequest request, CancellationToken cancellationToken = default);

    Task DeleteNoteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
