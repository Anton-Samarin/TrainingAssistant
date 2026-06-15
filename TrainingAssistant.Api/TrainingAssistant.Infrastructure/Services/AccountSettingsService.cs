using Microsoft.EntityFrameworkCore;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Settings;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Очистка недельных планов и полное удаление аккаунта
/// </summary>
public class AccountSettingsService : IAccountSettingsService
{
    private readonly ApplicationDbContext _db;

    public AccountSettingsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ClearWeeksResultDto> ClearAllWeeksAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var plans = await _db.WeeklyPlans.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        if (plans.Count == 0)
            return new ClearWeeksResultDto { DeletedCount = 0 };

        _db.WeeklyPlans.RemoveRange(plans);
        await _db.SaveChangesAsync(cancellationToken);

        return new ClearWeeksResultDto { DeletedCount = plans.Count };
    }

    public async Task DeleteAccountAsync(
        Guid userId,
        DeleteAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Укажите пароль для подтверждения.");

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Пользователь не найден.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Неверный пароль.");

        var plans = await _db.WeeklyPlans.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        _db.WeeklyPlans.RemoveRange(plans);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
