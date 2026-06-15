using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.DTOs.Auth;
using TrainingAssistant.Application.Options;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Infrastructure.Helpers;
using TrainingAssistant.Infrastructure.Persistence;

namespace TrainingAssistant.Infrastructure.Services;

/// <summary>
/// Регистрация, вход и создание профиля при регистрации
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly JwtTokenService _jwtTokenService;

    public AuthService(ApplicationDbContext db, JwtTokenService jwtTokenService)
    {
        _db = db;
        _jwtTokenService = jwtTokenService;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(x => x.Email == email, cancellationToken))
            throw new InvalidOperationException("Пользователь с таким email уже существует.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAtUtc = DateTime.UtcNow,
            Profile = new UserProfile
            {
                UserId = default,
                Sex = request.Sex,
                Age = request.Age,
                WeightKg = request.WeightKg,
                HeightCm = request.HeightCm,
                Goal = request.Goal,
                TrainingFocus = request.TrainingFocus,
                FitnessLevel = request.FitnessLevel,
                SessionsPerWeek = request.SessionsPerWeek,
                SessionDurationMin = request.SessionDurationMin,
                ActivityLevel = request.ActivityLevel,
                EquipmentJson = JsonListHelper.Serialize(request.Equipment),
                InjuriesJson = JsonListHelper.Serialize(request.Injuries),
                UpdatedAtUtc = DateTime.UtcNow
            }
        };
        user.Profile!.UserId = user.Id;

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Неверный email или пароль.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Неверный email или пароль.");

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var token = _jwtTokenService.CreateToken(user.Id, user.Email);
        return new AuthResponse { Token = token, UserId = user.Id, Name = user.Name, Email = user.Email };
    }
}
