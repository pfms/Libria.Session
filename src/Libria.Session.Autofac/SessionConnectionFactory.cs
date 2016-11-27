using Autofac;
using Libria.Session.Interfaces;

namespace Libria.Session.Autofac
{
	public class SessionConnectionFactory : ISessionConnectionFactory
	{
		protected readonly IComponentContext Context;

		public SessionConnectionFactory(IComponentContext context)
		{
			Context = context;
		}

		public virtual TSessionConnection CreateSessionConnection<TSessionConnection>()
			where TSessionConnection : class, ISessionConnection
		{
			return Context.Resolve<TSessionConnection>();
		}
	}
}