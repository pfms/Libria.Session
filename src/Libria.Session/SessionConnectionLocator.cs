using Libria.Session.Interfaces;

namespace Libria.Session
{
	public class SessionConnectionLocator : ISessionConnectionLocator
	{
		public TSessionConnection Get<TSessionConnection>() where TSessionConnection : class, ISessionConnection
		{
			var ambientSessionConnection = SessionScope.Current;
			return ambientSessionConnection.SessionConnections.Get<TSessionConnection>();
		}
	}
}