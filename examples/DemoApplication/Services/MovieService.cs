using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DemoApplication.Data.Entities;
using DemoApplication.Data.Repositories;
using DemoApplication.Services.Models;
using Libria.AmbientContext;
using Libria.Session.Aspects;
using Libria.Session.Interfaces;

namespace DemoApplication.Services
{
	public class MovieService
	{
		private readonly MovieRepository _movies;
		private readonly ISessionScopeFactory _scopeFactory;

		public MovieService(ISessionScopeFactory scopeFactory,
			MovieRepository movies)
		{
			_scopeFactory = scopeFactory;
			_movies = movies;
		}

		[SessionScope(ReadOnly = true)]
		public virtual async Task<MovieModel> FindByNameAsync(string name, CancellationToken ct)
		{
			//using (var scope = _scopeFactory.Create(true))
			{
				var existingMovie = await _movies.FindByNameAsync(name, ct);

				return Mapper.Map<MovieModel>(existingMovie);
			}
		}

		[SessionScope]
		public virtual async Task<MovieModel> AddAsync(MovieModel movieModel, CancellationToken ct)
		{
			//using (var scope = _scopeFactory.Create())
			{
				var movie = await _movies.AddAsync(Mapper.Map<Movie>(movieModel), ct);
				//await scope.SaveChangesAsync(ct);
				return Mapper.Map<MovieModel>(movie);
			}
		}

		[SessionScope]
		public virtual async Task<MovieModel> AddOrUpdateAsync(MovieModel movieModel, CancellationToken ct)
		{
			//using (var scope = _scopeFactory.Create())
			{
				var existingModel = await FindByNameAsync(movieModel.Name, ct);

				if (existingModel != null)
				{
					return await UpdateAsync(movieModel, ct);
				}

				//await scope.SaveChangesAsync(ct);
				return await AddAsync(movieModel, ct);
			}
		}

		[SessionScope]
		public virtual async Task<MovieModel> UpdateAsync(MovieModel movieModel, CancellationToken ct)
		{
			//using (var scope = _scopeFactory.Create())
			{
				var movie = await _movies.UpdateAsync(Mapper.Map<Movie>(movieModel), ct);
				//await scope.SaveChangesAsync(ct);
				return Mapper.Map<MovieModel>(movie);
			}
		}
	}
}