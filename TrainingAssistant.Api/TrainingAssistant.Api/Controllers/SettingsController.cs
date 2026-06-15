using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Settings;

namespace TrainingAssistant.Api.Controllers;

/// <summary>
/// Настройки аккаунта и данных пользователя
/// </summary>
[ApiController]
[Authorize]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly IAccountSettingsService _settings;

    public SettingsController(IAccountSettingsService settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Удаляет все сохранённые недельные планы пользователя
    /// </summary>
    [HttpDelete("weeks")]
    public async Task<ActionResult<ClearWeeksResultDto>> ClearWeeks(CancellationToken cancellationToken)
    {
        return Ok(await _settings.ClearAllWeeksAsync(GetUserId(), cancellationToken));
    }

    /// <summary>
    /// Полностью удаляет аккаунт и связанные данные
    /// </summary>
    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount(
        [FromBody] DeleteAccountRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _settings.DeleteAccountAsync(GetUserId(), request, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
}
