using TrainingAssistant.Application.DTOs.Settings;

namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Настройки аккаунта: очистка данных и удаление пользователя
/// </summary>
public interface IAccountSettingsService
{
    Task<ClearWeeksResultDto> ClearAllWeeksAsync(Guid userId, CancellationToken cancellationToken = default);

    Task DeleteAccountAsync(Guid userId, DeleteAccountRequest request, CancellationToken cancellationToken = default);
}
