using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Plans;

namespace TrainingAssistant.Api.Controllers;

/// <summary>
/// Недельные планы питания и тренировок
/// </summary>
[ApiController]
[Authorize]
[Route("api/plans")]
public class PlansController : ControllerBase
{
    private readonly IWeekPlanService _weekPlanService;

    public PlansController(IWeekPlanService weekPlanService)
    {
        _weekPlanService = weekPlanService;
    }

    [HttpPost("week/generate")]
    public async Task<ActionResult<WeekPlanDto>> GenerateWeek(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _weekPlanService.GenerateWeekAsync(GetUserId(), cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("week/current")]
    public async Task<ActionResult<WeekPlanDto>> GetCurrentWeek(CancellationToken cancellationToken)
    {
        var plan = await _weekPlanService.GetCurrentWeekAsync(GetUserId(), cancellationToken);
        return plan is null ? NotFound(new { message = "Активный план на неделю не найден." }) : Ok(plan);
    }

    [HttpGet("exercises/alternatives")]
    public ActionResult<IReadOnlyList<string>> GetAlternatives([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Укажите name текущего упражнения." });
        return Ok(_weekPlanService.GetExerciseAlternatives(name.Trim()));
    }

    [HttpPatch("exercises/{exerciseId:guid}")]
    public async Task<ActionResult<WeekPlanDto>> ReplaceExercise(
        Guid exerciseId,
        [FromBody] ReplaceExerciseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _weekPlanService.ReplaceExerciseAsync(GetUserId(), exerciseId, request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("training-days/{trainingDayId:guid}/intensity")]
    public async Task<ActionResult<WeekPlanDto>> AdjustDay(
        Guid trainingDayId,
        [FromBody] AdjustTrainingDayRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _weekPlanService.AdjustTrainingDayAsync(GetUserId(), trainingDayId, request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("exercises/{exerciseId:guid}/completed")]
    public async Task<ActionResult<WeekPlanDto>> SetExerciseCompleted(
        Guid exerciseId,
        [FromBody] SetCompletedRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _weekPlanService.SetExerciseCompletedAsync(GetUserId(), exerciseId, request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("meal-items/{mealItemId:guid}/completed")]
    public async Task<ActionResult<WeekPlanDto>> SetMealItemCompleted(
        Guid mealItemId,
        [FromBody] SetCompletedRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _weekPlanService.SetMealItemCompletedAsync(GetUserId(), mealItemId, request, cancellationToken));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
}
