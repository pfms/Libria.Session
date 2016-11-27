using Autofac;
using Autofac.Extras.DynamicProxy;
using DemoApplication.Data;
using Libria.Session.Autofac.Interceptors;

namespace DemoApplication.IoC
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SessionScopeInterceptor>()
				.AsSelf()
				.InstancePerDependency();

			builder.RegisterAssemblyTypes(typeof (MovieDbContext).Assembly)
				.Where(t => t.Name.EndsWith("Service"))
				.AsSelf()
				.InstancePerDependency()
				.EnableClassInterceptors()
				.InterceptedBy(typeof(SessionScopeInterceptor));
		}
	}
}