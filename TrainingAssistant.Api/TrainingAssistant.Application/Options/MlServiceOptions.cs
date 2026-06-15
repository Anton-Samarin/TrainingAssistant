namespace TrainingAssistant.Application.Options;

/// <summary>
/// Настройки HTTP-клиента ML-сервиса
/// </summary>
public class MlServiceOptions
{
    public const string SectionName = "MlService";

    public string BaseUrl { get; set; } = "http://localhost:8000";
    public string ApiKey { get; set; } = "dev-ml-key";
    public int TimeoutSeconds { get; set; } = 120;
}
