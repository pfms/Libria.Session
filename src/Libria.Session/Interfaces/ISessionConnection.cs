using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Libria.Session.Interfaces
{
	public interface ISessionConnection : IDisposable
	{
		IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
		int SaveChanges();
		Task<int> SaveChangesAsync(CancellationToken ct);
	}
}