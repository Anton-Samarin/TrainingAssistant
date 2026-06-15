namespace TrainingAssistant.Application.Options;

/// <summary>
/// Фоновое продление недельных планов по истечении 7-дневного периода
/// </summary>
public class WeekRenewalOptions
{
    public const string SectionName = "WeekRenewal";

    public bool Enabled { get; set; } = true;

    /// <summary>Интервал проверки в минутах</summary>
    public int IntervalMinutes { get; set; } = 60;
}
