using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DemoApplication.Data.Entities;
using DemoApplication.Data.Repositories;
using DemoApplication.IoC;
using DemoApplication.Services;
using DemoApplication.Services.Models;
using Libria.Session.Interfaces;

namespace DemoApplication
{
	class Program
	{
		private readonly MovieService _movieService;
		private static IContainer container;

		static void Main(string[] args)
		{
			Mapper.Initialize(c =>
			{
				c.CreateMissingTypeMaps = true;
			});

			var builder = new ContainerBuilder();

			builder.RegisterModule<DataModule>();
			builder.RegisterModule<ServiceModule>();

			builder.RegisterType<Program>()
				.AsSelf()
				.SingleInstance();

			container = builder.Build();
			container.Resolve<Program>().Run(CancellationToken.None).Wait();
		}

		public Program(MovieService movieService)
		{
			_movieService = movieService;
		}

		public async Task Run(CancellationToken ct)
		{
			var movie = new MovieModel()
			{
				Id = Guid.NewGuid(),
				Name = "Star Wars: Episode VI - Return of the Jedi",
				ImdbUrl = "http://www.imdb.com/title/tt0086190/",
				UpdatedOn = DateTimeOffset.Now
			};

			//movie = await _movieService.FindByNameAsync(movie.Name, ct);
			movie = await _movieService.AddOrUpdateAsync(movie, ct);
		}
	}
}
