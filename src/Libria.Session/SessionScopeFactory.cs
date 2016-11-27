using System.Data;
using Libria.AmbientContext;
using Libria.Session.Interfaces;

namespace Libria.Session
{
	public class SessionScopeFactory : ISessionScopeFactory
	{
		private readonly ISessionConnectionFactory _factory;
		private readonly ISessionExceptionHandler _exceptionHandler;

		public SessionScopeFactory(ISessionConnectionFactory factory, ISessionExceptionHandler exceptionHandler)
		{
			_factory = factory;
			_exceptionHandler = exceptionHandler;
		}

		public ISessionScope Create(bool readOnly = false, ScopeOption joiningOption = ScopeOption.Required)
		{
			return new SessionScope(readOnly, null, _factory, joiningOption, _exceptionHandler);
		}

		public ISessionScope CreateWithTransaction(bool readOnly = false,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
		{
			return new SessionScope(
				readOnly,
				isolationLevel,
				_factory,
				ScopeOption.RequiresNew,
				_exceptionHandler);
		}

		public ISessionScope CreateSuppressed(bool readOnly = false,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
		{
			return new SessionScope(
				readOnly,
				isolationLevel,
				_factory,
				ScopeOption.Suppress,
				_exceptionHandler);
		}
	}
}