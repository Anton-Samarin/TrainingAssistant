namespace TrainingAssistant.Application.DTOs.Progress;

public class BodyMetricLogDto
{
    public Guid Id { get; set; }
    public DateOnly LogDate { get; set; }
    public decimal WeightKg { get; set; }
    public string? Note { get; set; }
}

public class CreateBodyMetricRequest
{
    public DateOnly LogDate { get; set; }
    public decimal WeightKg { get; set; }
    public string? Note { get; set; }
}

public class StrengthRecordDto
{
    public Guid Id { get; set; }
    public DateOnly RecordDate { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public string? Note { get; set; }
}

public class CreateStrengthRecordRequest
{
    public DateOnly RecordDate { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public string? Note { get; set; }
}

public class ProgressNoteDto
{
    public Guid Id { get; set; }
    public DateOnly NoteDate { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class CreateProgressNoteRequest
{
    public DateOnly NoteDate { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class ProgressOverviewDto
{
    public List<BodyMetricLogDto> WeightLogs { get; set; } = new();
    public List<StrengthRecordDto> StrengthRecords { get; set; } = new();
    public List<ProgressNoteDto> Notes { get; set; } = new();
}
