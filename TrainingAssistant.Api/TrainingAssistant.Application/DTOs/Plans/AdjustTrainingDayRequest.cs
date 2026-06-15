namespace TrainingAssistant.Application.DTOs.Plans;

/// <summary>
/// Корректировка нагрузки тренировочного дня
/// </summary>
public class AdjustTrainingDayRequest
{
    /// <summary>easier или harder</summary>
    public string Mode { get; set; } = "easier";
}
