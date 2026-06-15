using Microsoft.EntityFrameworkCore;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Progress;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

public class ProgressService : IProgressService
{
    private readonly ApplicationDbContext _db;
    private readonly IHealthAssessmentService _health;

    public ProgressService(ApplicationDbContext db, IHealthAssessmentService health)
    {
        _db = db;
        _health = health;
    }

    public async Task<ProgressOverviewDto> GetOverviewAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var weights = await _db.BodyMetricLogs.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LogDate)
            .Take(52)
            .ToListAsync(cancellationToken);

        var strength = await _db.StrengthRecords.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.RecordDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .ToListAsync(cancellationToken);

        var notes = await _db.ProgressNotes.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.NoteDate)
            .Take(52)
            .ToListAsync(cancellationToken);

        return new ProgressOverviewDto
        {
            WeightLogs = weights.OrderBy(x => x.LogDate).Select(MapWeight).ToList(),
            StrengthRecords = strength.Select(MapStrength).ToList(),
            Notes = notes.Select(MapNote).ToList()
        };
    }

    public async Task<WeightProfileSyncDto> AddWeightLogAsync(
        Guid userId,
        CreateBodyMetricRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = new BodyMetricLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LogDate = request.LogDate,
            WeightKg = request.WeightKg,
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.BodyMetricLogs.Add(entity);

        var profile = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Профиль не найден.");

        profile.WeightKg = request.WeightKg;
        profile.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return BuildWeightSync(profile, MapWeight(entity));
    }

    public async Task<WeightProfileSyncDto> DeleteWeightLogAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.BodyMetricLogs.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Запись не найдена.");
        _db.BodyMetricLogs.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);

        var profile = await _db.UserProfiles.FirstAsync(x => x.UserId == userId, cancellationToken);
        await ApplyLatestLogWeightToProfileAsync(userId, profile, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return BuildWeightSync(profile, log: null);
    }

    private async Task ApplyLatestLogWeightToProfileAsync(
        Guid userId,
        UserProfile profile,
        CancellationToken cancellationToken)
    {
        var latest = await _db.BodyMetricLogs
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LogDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is not null)
        {
            profile.WeightKg = latest.WeightKg;
            profile.UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    private WeightProfileSyncDto BuildWeightSync(UserProfile profile, BodyMetricLogDto? log)
    {
        var bmi = _health.CalculateBmi(profile.WeightKg, profile.HeightCm);
        return new WeightProfileSyncDto
        {
            Log = log,
            ProfileWeightKg = profile.WeightKg,
            Bmi = bmi,
            BmiCategory = _health.GetBmiCategory(bmi)
        };
    }

    public async Task<StrengthRecordDto> AddStrengthRecordAsync(
        Guid userId,
        CreateStrengthRecordRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = new StrengthRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RecordDate = request.RecordDate,
            ExerciseName = request.ExerciseName.Trim(),
            WeightKg = request.WeightKg,
            Reps = request.Reps,
            Sets = request.Sets,
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.StrengthRecords.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return MapStrength(entity);
    }

    public async Task DeleteStrengthRecordAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.StrengthRecords.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Запись не найдена.");
        _db.StrengthRecords.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProgressNoteDto> AddNoteAsync(
        Guid userId,
        CreateProgressNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = new ProgressNote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NoteDate = request.NoteDate,
            Text = request.Text.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.ProgressNotes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return MapNote(entity);
    }

    public async Task DeleteNoteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.ProgressNotes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Заметка не найдена.");
        _db.ProgressNotes.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static BodyMetricLogDto MapWeight(BodyMetricLog x) => new()
    {
        Id = x.Id,
        LogDate = x.LogDate,
        WeightKg = x.WeightKg,
        Note = x.Note
    };

    private static StrengthRecordDto MapStrength(StrengthRecord x) => new()
    {
        Id = x.Id,
        RecordDate = x.RecordDate,
        ExerciseName = x.ExerciseName,
        WeightKg = x.WeightKg,
        Reps = x.Reps,
        Sets = x.Sets,
        Note = x.Note
    };

    private static ProgressNoteDto MapNote(ProgressNote x) => new()
    {
        Id = x.Id,
        NoteDate = x.NoteDate,
        Text = x.Text
    };
}
