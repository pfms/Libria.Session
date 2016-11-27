using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Libria.Session.Interfaces;

namespace Libria.Session
{
	public class SessionConnectionCollection
	{
		private readonly ISessionConnectionFactory _factory;
		private readonly ISessionExceptionHandler _exceptionHandler;
		private IsolationLevel? _isolationLevel;
		private readonly bool _readOnly;
		private bool _disposed;
		private bool _completed;

		private readonly Dictionary<ISessionConnection, IDbTransaction> _transactions;

		public SessionConnectionCollection(bool readOnly, IsolationLevel? isolationLevel, ISessionConnectionFactory factory, ISessionExceptionHandler exceptionHandler)
		{
			_readOnly = readOnly;
			_isolationLevel = isolationLevel;
			_factory = factory;
			_exceptionHandler = exceptionHandler;
			InitializedConnections = new Dictionary<Type, ISessionConnection>();
			_transactions = new Dictionary<ISessionConnection, IDbTransaction>();
		}

		internal Dictionary<Type, ISessionConnection> InitializedConnections { get; }

		public TSessionConnection Get<TSessionConnection>() where TSessionConnection : class, ISessionConnection
		{
			if (_disposed)
				throw new ObjectDisposedException("SessionConnectionCollection");

			var requestedType = typeof(TSessionConnection);

			if (!InitializedConnections.ContainsKey(requestedType))
			{
				var connection = _factory.CreateSessionConnection<TSessionConnection>();
				InitializedConnections.Add(requestedType, connection);

				if (_isolationLevel != null)
				{
					var tran = connection.BeginTransaction(_isolationLevel.Value);
					_transactions.Add(connection, tran);
				}
			}

			return InitializedConnections[requestedType] as TSessionConnection;
		}

		public void Dispose()
		{
			if (_disposed)
				return;

			if (!_completed)
			{
				try
				{
					if (_readOnly)
					{
						Commit();
					}
					else
					{
						Rollback();
					}
				}
				catch (Exception e)
				{
					_exceptionHandler?.HandleException(e);
				}
			}

			foreach (var conn in InitializedConnections.Values)
			{
				try
				{
					conn.Dispose();
				}
				catch (Exception e)
				{
					_exceptionHandler?.HandleException(e);
				}
			}

			InitializedConnections.Clear();
			_disposed = true;
		}

		public async Task<int> CommitAsync(CancellationToken ct)
		{
			if (_disposed)
				throw new ObjectDisposedException("SessionConnectionCollection");
			if (_completed)
				throw new InvalidOperationException("Not possible to call Commit or Rollback more than once on a SessionConnectionCollection");

			ExceptionDispatchInfo lastError = null;
			var c = 0;

			foreach (var conn in InitializedConnections.Values)
			{
				try
				{
					if (!_readOnly)
					{
						c += await conn.SaveChangesAsync(ct);
					}

					IDbTransaction tran;

					if (_transactions.TryGetValue(conn, out tran))
					{
						tran.Commit();
						tran.Dispose();
					}
				}
				catch (Exception e)
				{
					lastError = ExceptionDispatchInfo.Capture(e);
				}
			}

			_transactions.Clear();
			_completed = true;
			lastError?.Throw();

			return c;
		}

		public int Commit()
		{
			if (_disposed)
				throw new ObjectDisposedException("SessionConnectionCollection");
			if (_completed)
				throw new InvalidOperationException("Not possible to call Commit or Rollback more than once on a SessionConnectionCollection");

			ExceptionDispatchInfo lastError = null;
			var c = 0;

			foreach (var conn in InitializedConnections.Values)
			{
				try
				{
					if (!_readOnly)
					{
						c += conn.SaveChanges();
					}

					IDbTransaction tran;

					if (_transactions.TryGetValue(conn, out tran))
					{
						tran.Commit();
						tran.Dispose();
					}
				}
				catch (Exception e)
				{
					lastError = ExceptionDispatchInfo.Capture(e);
				}
			}

			_transactions.Clear();
			_completed = true;
			lastError?.Throw();

			return c;
		}

		public void Rollback()
		{
			if (_disposed)
				throw new ObjectDisposedException("SessionConnectionCollection");
			if (_completed)
				throw new InvalidOperationException("Not possible to call Commit or Rollback more than once on a SessionConnectionCollection");

			ExceptionDispatchInfo lastError = null;

			foreach (var conn in InitializedConnections.Values)
			{
				try
				{
					IDbTransaction tran;

					if (_transactions.TryGetValue(conn, out tran))
					{
						tran.Rollback();
						tran.Dispose();
					}
				}
				catch (Exception e)
				{
					lastError = ExceptionDispatchInfo.Capture(e);
				}
			}

			_transactions.Clear();
			_completed = true;
			lastError?.Throw();
		}
	}
}