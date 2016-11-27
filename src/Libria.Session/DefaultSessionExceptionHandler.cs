using System;
using System.Diagnostics;
using Libria.Session.Interfaces;

namespace Libria.Session
{
	public class DefaultSessionExceptionHandler : ISessionExceptionHandler
	{
		public void HandleException(Exception e)
		{
			Debug.WriteLine(e.ToString());
		}

		public static ISessionExceptionHandler Instance { get; } = new DefaultSessionExceptionHandler();
	}
}