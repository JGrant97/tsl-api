using tsl_api.Sessions;

public sealed class MockSessionService : ISessionService
{
    private readonly object _lock = new();
    private readonly Timer _timer;
    private readonly List<Session> _sessions;
    private DateTime _lastTickUtc;

    public MockSessionService()
    {
        _sessions = new()
        {
            new Session
            {
                Id = Guid.NewGuid(),
                Series = "GT World",
                Name = "Free Practice 1",
                Track = "Brands Hatch",
                State = SessionState.NotStarted,
                Duration = TimeSpan.MinValue
            },
            new Session
            {
                Id = Guid.NewGuid(),
                Series = "F3",
                Name = "Qualifying",
                Track = "Silverstone",
                State = SessionState.NotStarted,
                Duration = TimeSpan.MinValue
            }
        };

        _lastTickUtc = DateTime.UtcNow;

        // Fire every 1 second
        _timer = new Timer(_ => UpdateSessions(), null, dueTime: 0, period: 1000);
    }

    //The lock ensures that reading and writing the collection never happen at the same time
    public IEnumerable<Session> GetSnapshot()
    {
        lock (_lock)
            return _sessions.Select(Clone);
    }

    private void UpdateSessions()
    {
        lock (_lock)
        {
            foreach (var session in _sessions)
            {
                session.State = NextState(session.State);

                switch (session.State)
                {
                    case SessionState.GreenFlag:
                        // If session becomes active for the first time, set StartTime
                        session.StartTime ??= DateTime.UtcNow;
                        session.Duration += DateTime.UtcNow - _lastTickUtc;
                        break;
                    case SessionState.RedFlag:
                        break;
                }
            }
        }
    }

    private static SessionState NextState(SessionState s) => s switch
    {
        SessionState.NotStarted => SessionState.GreenFlag,
        SessionState.GreenFlag => SessionState.YellowFlag,
        SessionState.YellowFlag => SessionState.GreenFlag,
        SessionState.RedFlag => SessionState.GreenFlag,
        SessionState.Finished => SessionState.Finished,
        _ => SessionState.GreenFlag
    };

    private static Session Clone(Session s) => new()
    {
        Id = s.Id,
        Series = s.Series,
        Name = s.Name,
        Track = s.Track,
        State = s.State,
        StartTime = s.StartTime,
        Duration = s.Duration
    };
}