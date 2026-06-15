namespace TrainingAssistant.Application.DTOs.Plans;

/// <summary>
/// Отметка выполнения упражнения или приёма пищи
/// </summary>
public class SetCompletedRequest
{
    public bool IsCompleted { get; set; } = true;
}
