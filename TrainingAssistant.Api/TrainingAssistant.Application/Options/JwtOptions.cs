namespace TrainingAssistant.Application.Options;

/// <summary>
/// Настройки выпуска JWT-токенов
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "TrainingAssistant";
    public string Audience { get; set; } = "TrainingAssistant";
    public string Key { get; set; } = "CHANGE_ME_TO_A_LONG_SECRET_KEY_32_CHARS_MIN";
    public int ExpirationMinutes { get; set; } = 10080;
}
