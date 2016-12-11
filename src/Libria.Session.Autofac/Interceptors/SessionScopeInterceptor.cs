using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Libria.Session.Aspects;
using Libria.Session.Interfaces;

namespace Libria.Session.Autofac.Interceptors
{
	public class SessionScopeInterceptor : IInterceptor
	{
		private readonly ISessionScopeFactory _scopeFactory;

		public SessionScopeInterceptor(ISessionScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		public async void Intercept(IInvocation invocation)
		{
			var sessionScopeAttribute = invocation.MethodInvocationTarget
				.GetCustomAttributes(typeof (SessionScopeAttribute), true)
				.FirstOrDefault() as SessionScopeAttribute;

			if (sessionScopeAttribute != null)
			{
				var returnType = invocation.MethodInvocationTarget.ReturnType;
				if (returnType == typeof (Task) ||
				    returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof (Task<>))
				{
					var ctArgument = invocation.Arguments.FirstOrDefault(a => a is CancellationToken);

					var cancellationToken = (CancellationToken?)ctArgument ?? CancellationToken.None;
					var scope = _scopeFactory
							.Create(sessionScopeAttribute.ReadOnly, sessionScopeAttribute.Option,
								sessionScopeAttribute.IsolationLevel);

					invocation.Proceed();
					invocation.ReturnValue = InterceptAsync((dynamic) invocation.ReturnValue, scope, cancellationToken);
				}
				else
				{
					using (var scope = _scopeFactory
						.Create(sessionScopeAttribute.ReadOnly, sessionScopeAttribute.Option,
							sessionScopeAttribute.IsolationLevel))
					{
						invocation.Proceed();
						scope.SaveChanges();
					}
				}
			}
			else
			{
				invocation.Proceed();
			}
		}

		private async Task InterceptAsync(Task task, ISessionScope scope, CancellationToken ct)
		{
			await task.ContinueWith(async t =>
			{
				await scope.SaveChangesAsync(ct);
				scope.Dispose();
			}, ct).ConfigureAwait(false);
		}

		private async Task<T> InterceptAsync<T>(Task<T> task, ISessionScope scope, CancellationToken ct)
		{
			var result = await task.ContinueWith(async t =>
			{
				var res = t.Result;
				await scope.SaveChangesAsync(ct);
				scope.Dispose();

				return res;
			}, ct).ConfigureAwait(false);

			return await result;
		}
	}
}