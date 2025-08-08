namespace tsl_api.Sessions;

public interface ISessionService
{
    IEnumerable<Session> GetSnapshot();
}
