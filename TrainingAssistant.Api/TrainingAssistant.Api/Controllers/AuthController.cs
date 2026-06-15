using Microsoft.AspNetCore.Mvc;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Auth;

namespace TrainingAssistant.Api.Controllers;

/// <summary>
/// Регистрация и авторизация
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Создаёт аккаунт и профиль, отдаёт JWT
    /// </summary>
    /// <param name="request">Email, пароль и параметры профиля</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Токен и id пользователя</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _authService.RegisterAsync(request, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Вход по email и паролю
    /// </summary>
    /// <param name="request">Email и пароль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Токен и id пользователя</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _authService.LoginAsync(request, cancellationToken));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
