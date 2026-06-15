using Microsoft.EntityFrameworkCore;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Profile;
using TrainingAssistant.Infrastructure.Helpers;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Чтение и обновление профиля в базе данных
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly ApplicationDbContext _db;

    public UserProfileService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<ProfileDto> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking()
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Пользователь не найден.");

        if (user.Profile is null)
            throw new KeyNotFoundException("Профиль не найден. Сначала завершите регистрацию.");

        return Map(user.Profile, user.Name);
    }

    /// <inheritdoc />
    public async Task<ProfileDto> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Пользователь не найден.");

        var profile = user.Profile
            ?? throw new KeyNotFoundException("Профиль не найден.");

        user.Name = request.Name.Trim();
        profile.Sex = request.Sex;
        profile.Age = request.Age;
        profile.WeightKg = request.WeightKg;
        profile.HeightCm = request.HeightCm;
        profile.Goal = request.Goal;
        profile.TrainingFocus = request.TrainingFocus;
        profile.FitnessLevel = request.FitnessLevel;
        profile.SessionsPerWeek = request.SessionsPerWeek;
        profile.SessionDurationMin = request.SessionDurationMin;
        profile.ActivityLevel = request.ActivityLevel;
        profile.EquipmentJson = JsonListHelper.Serialize(request.Equipment);
        profile.InjuriesJson = JsonListHelper.Serialize(request.Injuries);
        profile.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Map(profile, user.Name);
    }

    private static ProfileDto Map(Domain.Entities.UserProfile profile, string name) => new()
    {
        Name = name,
        Sex = profile.Sex,
        Age = profile.Age,
        WeightKg = profile.WeightKg,
        HeightCm = profile.HeightCm,
        Goal = profile.Goal,
        TrainingFocus = profile.TrainingFocus,
        FitnessLevel = profile.FitnessLevel,
        SessionsPerWeek = profile.SessionsPerWeek,
        SessionDurationMin = profile.SessionDurationMin,
        ActivityLevel = profile.ActivityLevel,
        Equipment = JsonListHelper.Deserialize(profile.EquipmentJson),
        Injuries = JsonListHelper.Deserialize(profile.InjuriesJson)
    };
}
