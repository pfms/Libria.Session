using System.Data;
using Libria.AmbientContext;

namespace Libria.Session.Interfaces
{
	public interface ISessionScopeFactory
	{
		ISessionScope Create(bool readOnly = false, ScopeOption joiningOption = ScopeOption.Required);

		ISessionScope CreateWithTransaction(bool readOnly = false,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

		ISessionScope CreateSuppressed(bool readOnly = false,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
	}
}