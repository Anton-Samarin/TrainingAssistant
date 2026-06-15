using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Progress;

namespace TrainingAssistant.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progress;

    public ProgressController(IProgressService progress)
    {
        _progress = progress;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
        => Ok(await _progress.GetOverviewAsync(GetUserId(), cancellationToken));

    [HttpPost("weight")]
    public async Task<IActionResult> AddWeight([FromBody] CreateBodyMetricRequest request, CancellationToken cancellationToken)
        => Ok(await _progress.AddWeightLogAsync(GetUserId(), request, cancellationToken));

    [HttpDelete("weight/{id:guid}")]
    public async Task<IActionResult> DeleteWeight(Guid id, CancellationToken cancellationToken)
        => Ok(await _progress.DeleteWeightLogAsync(GetUserId(), id, cancellationToken));

    [HttpPost("strength")]
    public async Task<IActionResult> AddStrength([FromBody] CreateStrengthRecordRequest request, CancellationToken cancellationToken)
        => Ok(await _progress.AddStrengthRecordAsync(GetUserId(), request, cancellationToken));

    [HttpDelete("strength/{id:guid}")]
    public async Task<IActionResult> DeleteStrength(Guid id, CancellationToken cancellationToken)
    {
        await _progress.DeleteStrengthRecordAsync(GetUserId(), id, cancellationToken);
        return NoContent();
    }

    [HttpPost("notes")]
    public async Task<IActionResult> AddNote([FromBody] CreateProgressNoteRequest request, CancellationToken cancellationToken)
        => Ok(await _progress.AddNoteAsync(GetUserId(), request, cancellationToken));

    [HttpDelete("notes/{id:guid}")]
    public async Task<IActionResult> DeleteNote(Guid id, CancellationToken cancellationToken)
    {
        await _progress.DeleteNoteAsync(GetUserId(), id, cancellationToken);
        return NoContent();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
}
