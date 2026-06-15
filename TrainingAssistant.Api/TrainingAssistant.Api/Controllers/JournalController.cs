using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingAssistant.Application.Abstractions;

namespace TrainingAssistant.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/journal")]
public class JournalController : ControllerBase
{
    private readonly IJournalService _journal;

    public JournalController(IJournalService journal)
    {
        _journal = journal;
    }

    [HttpGet("weeks")]
    public async Task<IActionResult> ListWeeks([FromQuery] int limit = 30, CancellationToken cancellationToken = default)
    {
        return Ok(await _journal.ListWeeksAsync(GetUserId(), limit, cancellationToken));
    }

    [HttpGet("weeks/{planId:guid}")]
    public async Task<IActionResult> GetWeek(Guid planId, CancellationToken cancellationToken)
    {
        var plan = await _journal.GetWeekAsync(GetUserId(), planId, cancellationToken);
        return plan is null ? NotFound(new { message = "План не найден." }) : Ok(plan);
    }

    [HttpGet("day")]
    public async Task<IActionResult> GetDay(
        [FromQuery] DateOnly date,
        [FromQuery] Guid? planId,
        CancellationToken cancellationToken)
    {
        if (date == default)
            return BadRequest(new { message = "Укажите date в формате yyyy-MM-dd." });

        var day = await _journal.GetDayAsync(GetUserId(), date, planId, cancellationToken);
        return day is null ? NotFound(new { message = "За эту дату план не найден." }) : Ok(day);
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
}
