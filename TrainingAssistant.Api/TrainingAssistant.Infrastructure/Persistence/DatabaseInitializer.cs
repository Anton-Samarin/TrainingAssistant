using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrainingAssistant.Infrastructure.Seeding;

namespace TrainingAssistant.Infrastructure.Persistence;

/// <summary>
/// Инициализация базы при старте приложения
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Применяет миграции EF и наполняет справочник блюд при первом запуске
    /// </summary>
    /// <param name="services">Корневой провайдер сервисов приложения</param>
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        var connection = db.Database.GetDbConnection();
        logger.LogInformation(
            "Database target: {DataSource}, database={Database}",
            connection.DataSource,
            connection.Database);

        var applied = (await db.Database.GetAppliedMigrationsAsync()).ToList();
        var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();

        logger.LogInformation(
            "EF migrations in history ({Count}): {Applied}",
            applied.Count,
            applied.Count > 0 ? string.Join(", ", applied) : "(none)");

        if (pending.Count == 0)
        {
            logger.LogInformation("No pending migrations — schema is up to date.");
        }
        else
        {
            logger.LogInformation(
                "Pending migrations ({Count}): {Pending}",
                pending.Count,
                string.Join(", ", pending));
            await db.Database.MigrateAsync();
            logger.LogInformation("Pending migrations applied.");
        }

        var catalog = FoodDataSeeder.GetFoods();
        var existingIds = await db.FoodItems.Select(f => f.Id).ToListAsync();
        var missing = catalog.Where(f => !existingIds.Contains(f.Id)).ToList();
        if (missing.Count > 0)
        {
            db.FoodItems.AddRange(missing);
            await db.SaveChangesAsync();
            logger.LogInformation("Added {Count} food items to catalog ({Total} total).",
                missing.Count, await db.FoodItems.CountAsync());
        }
    }
}
