using Libria.Session.Repository.Interfaces;

namespace Libria.Session.EF6.Interfaces
{
	public interface IEFRepository<TEntity> : IRepository<TEntity> where TEntity : class
	{
	}
}