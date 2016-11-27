using System.Data.Entity;
using Libria.Session.Interfaces;

namespace Libria.Session.EF6.Interfaces
{
	public interface IDbContextSessionConnection<out TDbContext> : ISessionConnection
		where TDbContext : DbContext
	{
		TDbContext DbContext { get; }
	}
}