using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Libria.Session.EF6;
using Libria.Session.Interfaces;

namespace Libria.Session.Tests
{
	public class MockEFRepository<T, TDbContext> : EFRepository<T, TDbContext> 
		where T : class where TDbContext : DbContext
	{
		protected MockEFRepository(ISessionConnectionLocator sessionConnectionLocator) : base(sessionConnectionLocator)
		{
		}

		public override T Add(T entity)
		{
			return DbSet.Add(entity);
		}

		public override T Remove(T entity)
		{
			return DbSet.Remove(entity);
		}

		public override T Update(T entity)
		{
			return entity;
		}

		public override Task<T> GetAsync(CancellationToken ct, params object[] keys)
		{
			return Task.Run(() => Get(keys), ct);
		}
	}
}