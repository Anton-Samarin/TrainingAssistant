using TrainingAssistant.Application.DTOs.Auth;

namespace TrainingAssistant.Application.Abstractions;

/// <summary>
/// Сервис регистрации и входа
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрирует пользователя и возвращает токен
    /// </summary>
    /// <param name="request">Email, пароль и параметры профиля</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>JWT и данные пользователя</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет логин и выдаёт токен
    /// </summary>
    /// <param name="request">Email и пароль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>JWT и данные пользователя</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
