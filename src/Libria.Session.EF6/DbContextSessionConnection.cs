using System.Data;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Libria.Session.EF6.Interfaces;

namespace Libria.Session.EF6
{
	public class DbContextSessionConnection<TDbContext> : IDbContextSessionConnection<TDbContext>
		where TDbContext : DbContext
	{
		public DbContextSessionConnection(TDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public TDbContext DbContext { get; }

		public void Dispose()
		{
			DbContext.Dispose();
		}

		public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return DbContext.Database.BeginTransaction(isolationLevel).UnderlyingTransaction;
		}

		public int SaveChanges()
		{
			return DbContext.SaveChanges();
		}

		public Task<int> SaveChangesAsync(CancellationToken ct)
		{
			return DbContext.SaveChangesAsync(ct);
		}
	}
}