using TrainingAssistant.Application.DTOs.Profile;

namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Работа с профилем пользователя
/// </summary>
public interface IUserProfileService
{
    /// <summary>
    /// Возвращает профиль по id пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Параметры профиля</returns>
    Task<ProfileDto> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет параметры профиля
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="request">Новые значения полей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновлённый профиль</returns>
    Task<ProfileDto> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}
