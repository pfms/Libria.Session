using System.Configuration;
using Autofac;
using DemoApplication.Data;
using Libria.Session;
using Libria.Session.Autofac;
using Libria.Session.EF6;

namespace DemoApplication.IoC
{
	public class DataModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			// Register repositories
			builder.RegisterAssemblyTypes(typeof (MovieDbContext).Assembly)
				.Where(t => t.Name.EndsWith("Repository"))
				.AsSelf()
				.InstancePerDependency();

			builder.Register(r =>
			{
				var connStr = ConfigurationManager.ConnectionStrings["MovieConnectionString"].ConnectionString;
				return new MovieDbContext(connStr);
			}).AsImplementedInterfaces()
				.AsSelf()
				.InstancePerDependency();

			builder.RegisterType<DbContextSessionConnection<MovieDbContext>>()
				.AsSelf()
				.InstancePerDependency();

			builder.RegisterType<SessionConnectionLocator>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<SessionScopeFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<SessionConnectionFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}