using System.ComponentModel.DataAnnotations;

namespace TrainingAssistant.Application.DTOs.Plans;

/// <summary>
/// Замена упражнения в текущем плане
/// </summary>
public class ReplaceExerciseRequest
{
    [Required, MinLength(2), MaxLength(120)]
    public string NewName { get; set; } = string.Empty;

    public bool RememberForFuture { get; set; } = true;
}
