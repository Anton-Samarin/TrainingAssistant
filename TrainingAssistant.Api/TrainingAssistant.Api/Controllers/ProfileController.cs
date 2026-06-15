using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Profile;

namespace TrainingAssistant.Api.Controllers;

/// <summary>
/// Профиль текущего пользователя
/// </summary>
[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IUserProfileService _profileService;

    public ProfileController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Возвращает сохранённый профиль
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Профиль авторизованного пользователя</returns>
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> Get(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _profileService.GetAsync(GetUserId(), cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Сохраняет изменения профиля
    /// </summary>
    /// <param name="request">Обновляемые поля профиля</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Сохранённый профиль</returns>
    [HttpPut]
    public async Task<ActionResult<ProfileDto>> Update([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _profileService.UpdateAsync(GetUserId(), request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
}
