using System.Threading;
using System.Threading.Tasks;
using Libria.AmbientContext.Interfaces;

namespace Libria.Session.Interfaces
{
	public interface ISessionScope : IScope
	{
		Task<int> SaveChangesAsync(CancellationToken ct);
		int SaveChanges();
	}
}