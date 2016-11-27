using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Libria.AmbientContext;
using Libria.Session.Interfaces;

namespace Libria.Session
{
	public class SessionScope : BaseScope<SessionScope, SessionConnectionCollection>, ISessionScope
	{		 
		private readonly bool _readOnly;
		private readonly ISessionExceptionHandler _exceptionHandler;
		private bool _completed;

		public SessionScope(
			bool readOnly,
			IsolationLevel? isolationLevel,
			ISessionConnectionFactory factory,
			ScopeOption option = ScopeOption.Required,
			ISessionExceptionHandler exceptionHandler = null) : 
			base(new SessionConnectionCollection(readOnly, isolationLevel, factory, exceptionHandler), option)
		{
			_completed = false;
			_readOnly = readOnly;
			_exceptionHandler = exceptionHandler;

			if (isolationLevel != null && option == ScopeOption.Required)
			{
				throw new ArgumentException(
					"Can't join an ambient session if an explicit database transaction is required");
			}

			if (Nested && option == ScopeOption.Required)
			{
				var parentScope = ParentScope;

				if (parentScope._readOnly && !_readOnly)
				{
					throw new InvalidOperationException(
						"Cannot nest a read/write session scope within a read-only scope");
				}
			}
		}

		internal SessionConnectionCollection SessionConnections => ScopeData;

		public int SaveChanges()
		{
			if (Disposed)
				throw new ObjectDisposedException("SessionScope");

			if (_completed)
				throw new InvalidOperationException("Not possible to save changes more than once");

			var c = 0;
			if (!Nested)
			{
				c = ScopeData.Commit();
			}

			_completed = true;
			return c;

		}

		public async Task<int> SaveChangesAsync(CancellationToken ct)
		{
			if (Disposed)
				throw new ObjectDisposedException("SessionScope");

			if (_completed)
				throw new InvalidOperationException("Not possible to save changes more than once");

			var c = 0;
			if (!Nested)
			{
				c = await ScopeData.CommitAsync(ct);
			}

			_completed = true;
			return c;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (!Nested)
			{
				if (!_completed)
				{
					try
					{
						if (_readOnly)
						{
							ScopeData.Commit();
						}
						else
						{
							ScopeData.Rollback();
						}
					}
					catch (Exception e)
					{
						_exceptionHandler?.HandleException(e);
					}
				}

				_completed = true;
				ScopeData.Dispose();
			}
		}
	}
}