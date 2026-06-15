using System.Text.Json.Serialization;

namespace TrainingAssistant.Infrastructure.Ml;

public class MlExercisePreference
{
    [JsonPropertyName("avoid_name")]
    public string AvoidName { get; set; } = string.Empty;

    [JsonPropertyName("preferred_name")]
    public string PreferredName { get; set; } = string.Empty;

    [JsonPropertyName("pool_key")]
    public string? PoolKey { get; set; }
}
