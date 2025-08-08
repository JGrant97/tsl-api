namespace tsl_api.Sessions;

public class Session()
{
    public Guid Id { get; set; }

    public required string Series { get; set; }

    public required string Name { get; set; }

    public required string Track { get; set; }

    public required SessionState State { get; set; }

    public DateTime? StartTime { get; set; }

    public required TimeSpan Duration { get; set; }
}
