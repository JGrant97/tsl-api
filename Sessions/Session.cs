using System.Text.Json.Serialization;

namespace tsl_api.Sessions;

public class Session()
{
    public Guid Id { get; set; }

    public required string Series { get; set; }

    public required string Name { get; set; }

    public required string Track { get; set; }

    public required SessionState State { get; set; }

    public DateTime? StartTime { get; set; }

    [JsonIgnore]
    public TimeSpan Duration { get; set; }

    [JsonPropertyName("Duration")]
    public string DurationFormatted => 
        Duration.ToString(@"hh\:mm\:ss");
}
