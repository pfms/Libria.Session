using System;
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
			var existingMovie = await _movies.FindByNameAsync(name, ct);

			return Mapper.Map<MovieModel>(existingMovie);
		}

		[SessionScope]
		public virtual async Task<MovieModel> AddAsync(MovieModel movieModel, CancellationToken ct)
		{
			var movie = await _movies.AddAsync(Mapper.Map<Movie>(movieModel), ct);
			return Mapper.Map<MovieModel>(movie);
		}

		[SessionScope]
		public virtual async Task<MovieModel> AddOrUpdateAsync(MovieModel movieModel, CancellationToken ct)
		{
			var existingModel = await FindByNameAsync(movieModel.Name, ct);

			if (existingModel != null)
			{
				existingModel.UpdatedOn = movieModel.UpdatedOn;
				movieModel = await UpdateAsync(existingModel, ct);
			}
			else
			{
				movieModel = await AddAsync(movieModel, ct);
			}

			return movieModel;
		}

		[SessionScope]
		public virtual async Task<MovieModel> UpdateAsync(MovieModel movieModel, CancellationToken ct)
		{
			var existingMovie = await _movies.GetAsync(ct, movieModel.Id);
			if (existingMovie == null) throw new ArgumentNullException(nameof(existingMovie));

			existingMovie.Name = movieModel.Name;
			existingMovie.ImdbUrl = movieModel.ImdbUrl;
			existingMovie.UpdatedOn = movieModel.UpdatedOn;

			var movie = await _movies.UpdateAsync(existingMovie, ct);
			return Mapper.Map<MovieModel>(movie);
		}
	}
}