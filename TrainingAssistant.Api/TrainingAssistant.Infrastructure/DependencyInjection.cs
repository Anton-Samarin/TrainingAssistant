using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainingAssistant.Application.Abstractions;
using TrainingAssistant.Application.Options;
using TrainingAssistant.Infrastructure.Ml;
using TrainingAssistant.Infrastructure.Persistence;
using TrainingAssistant.Infrastructure.Services;

namespace TrainingAssistant.Infrastructure;

/// <summary>
/// Расширения регистрации слоя Infrastructure в DI
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует БД, сервисы и клиент ML в DI
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения</param>
    /// <param name="configuration">Конфигурация с connection string и ML</param>
    /// <returns>Та же коллекция для цепочки вызовов</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<MlServiceOptions>(configuration.GetSection(MlServiceOptions.SectionName));
        services.Configure<WeekRenewalOptions>(configuration.GetSection(WeekRenewalOptions.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IWeekPlanService, WeekPlanService>();
        services.AddScoped<IWeekPlanRenewalService, WeekPlanRenewalService>();
        services.AddHostedService<WeeklyPlanRenewalBackgroundService>();
        services.AddScoped<IHealthAssessmentService, HealthAssessmentService>();
        services.AddScoped<IJournalService, JournalService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IAccountSettingsService, AccountSettingsService>();
        services.AddScoped<INutritionCalculator, NutritionCalculator>();
        services.AddScoped<NutritionPlanBuilder>();
        services.AddScoped<JwtTokenService>();

        var mlOptions = configuration.GetSection(MlServiceOptions.SectionName).Get<MlServiceOptions>() ?? new MlServiceOptions();
        services.AddHttpClient<MlTrainingClient>(client =>
        {
            client.BaseAddress = new Uri(mlOptions.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(mlOptions.TimeoutSeconds);
        });

        return services;
    }
}
