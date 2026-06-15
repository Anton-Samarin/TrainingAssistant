namespace TrainingAssistant.Application.DTOs.Progress;

/// <summary>
/// Результат записи веса с обновлённым профилем и ИМТ
/// </summary>
public class WeightProfileSyncDto
{
    public BodyMetricLogDto? Log { get; set; }
    public decimal ProfileWeightKg { get; set; }
    public double Bmi { get; set; }
    public string BmiCategory { get; set; } = string.Empty;
}
