using System;

namespace Libria.Session.Interfaces
{
	public interface ISessionExceptionHandler
	{
		void HandleException(Exception e);
	}
}