using System.Threading;
using System.Threading.Tasks;

namespace Libria.Session.Repository.Interfaces
{
	public interface IRepository<TEntity> where TEntity : class
	{
		TEntity Add(TEntity entity);
		TEntity Remove(TEntity entity);
		TEntity Update(TEntity entity);
		TEntity Get(params object[] keys);

		Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);
		Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct);
		Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct);
		Task<TEntity> GetAsync(CancellationToken ct, params object[] keys);
	}
}