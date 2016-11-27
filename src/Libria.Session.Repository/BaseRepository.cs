using System.Threading;
using System.Threading.Tasks;
using Libria.Session.Interfaces;
using Libria.Session.Repository.Interfaces;

namespace Libria.Session.Repository
{
	public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		protected BaseRepository(ISessionConnectionLocator sessionConnectionLocator)
		{
			SessionConnectionLocator = sessionConnectionLocator;
		}

		protected ISessionConnectionLocator SessionConnectionLocator { get; }
		public abstract TEntity Add(TEntity entity);
		public abstract TEntity Remove(TEntity entity);
		public abstract TEntity Update(TEntity entity);
		public abstract TEntity Get(params object[] keys);
		public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);
		public abstract Task<TEntity> GetAsync(CancellationToken ct, params object[] keys);
		public abstract Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct);
		public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct);
	}
}