using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrainingAssistant.Infrastructure;
using TrainingAssistant.Infrastructure.Persistence;

var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});
host.Services.AddInfrastructure(host.Configuration);
host.Services.AddLogging(builder => builder.AddConsole());

using var app = host.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TrainingAssistant.Migrator");

try
{
    await DatabaseInitializer.InitializeAsync(app.Services);
    return 0;
}
catch (Exception ex)
{
    logger.LogError(ex, "Migration failed.");
    return 1;
}
