namespace Libria.Session.Interfaces
{
	public interface ISessionConnectionFactory
	{
		TSessionConnection CreateSessionConnection<TSessionConnection>() where TSessionConnection : class, ISessionConnection;
	}
}