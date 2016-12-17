using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Libria.Session.Interfaces;
using Libria.Session.Repository;

namespace Libria.Session.EF6
{
	public class EFRepository<TEntity, TDbContext> : BaseRepository<TEntity> where TEntity : class
		where TDbContext : DbContext
	{
		protected EFRepository(ISessionConnectionLocator sessionConnectionLocator)
			: base(sessionConnectionLocator)
		{
		}

		protected TDbContext DbContext => SessionConnectionLocator
			.Get<DbContextSessionConnection<TDbContext>>()
			.DbContext;

		protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

		public override TEntity Add(TEntity entity)
		{
			var entry = DbContext.Entry(entity);

			if (entry.State == EntityState.Detached)
			{
				return DbSet.Add(entity);
			}

			entry.State = EntityState.Added;
			return entry.Entity;
		}

		public override TEntity Remove(TEntity entity)
		{
			var entry = DbContext.Entry(entity);

			if (entry.State != EntityState.Deleted)
			{
				entry.State = EntityState.Deleted;
				return entry.Entity;
			}

			if (entry.State == EntityState.Detached)
			{
				DbSet.Attach(entity);
			}

			DbSet.Remove(entity);

			return entity;
		}

		public override TEntity Update(TEntity entity)
		{
			var entry = DbContext.Entry(entity);

			if (entry.State == EntityState.Detached)
			{
				DbSet.Attach(entity);
			}

			if (entry.State != EntityState.Added && entry.State != EntityState.Deleted)
				entry.State = EntityState.Modified;

			return entry.Entity;
		}

		public override TEntity Get(params object[] keys)
		{
			return DbSet.Find(keys);
		}

		public override Task<TEntity> AddAsync(TEntity entity, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			return Task.FromResult(Add(entity));
		}

		public override Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			return Task.FromResult(Remove(entity));
		}

		public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			return Task.FromResult(Update(entity));
		}

		public override Task<TEntity> GetAsync(CancellationToken ct, params object[] keys)
		{
			return DbSet.FindAsync(ct, keys);
		}

		protected Task<T> QueryAsync<T>(Func<DbSet<TEntity>, T> query, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			return Task.FromResult(query(DbSet));
		}

		protected async Task<T> QueryAsync<T>(Func<DbSet<TEntity>, CancellationToken, Task<T>> query, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			var result = await query(DbSet, ct);
			return result;
		}
	}
}