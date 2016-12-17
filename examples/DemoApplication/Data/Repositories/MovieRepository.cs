using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DemoApplication.Data.Entities;
using Libria.Session.EF6;
using Libria.Session.Interfaces;

namespace DemoApplication.Data.Repositories
{
	public class MovieRepository : EFRepository<Movie, MovieDbContext>
	{
		public MovieRepository(ISessionConnectionLocator sessionConnectionLocator)
			: base(sessionConnectionLocator)
		{
		}
		
		public Task<Movie> FindByNameAsync(string name, CancellationToken ct)
		{
			return DbSet
				.FirstOrDefaultAsync(q => q.Name == name, ct);
		}

		public Task<bool> ExistsAsync(string name, CancellationToken ct)
		{
			return QueryAsync((d, c) => d.AnyAsync(p => p.Name == name, c), ct);
		}
	}
}