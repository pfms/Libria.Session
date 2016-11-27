namespace Libria.Session.Interfaces
{
	public interface ISessionConnectionLocator
	{
		TSessionConnection Get<TSessionConnection>() where TSessionConnection : class, ISessionConnection;
	}
}