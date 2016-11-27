using System;
using System.Data;
using Libria.AmbientContext;
using Libria.Session.Interfaces;

namespace Libria.Session.Aspects
{
	[AttributeUsage(AttributeTargets.Method)]
	public class SessionScopeAttribute : Attribute
	{
		public bool ReadOnly { get; set; }
		public IsolationLevel? IsolationLevel { get; set; }
		public ScopeOption Option { get; set; }

		public SessionScopeAttribute(ScopeOption option = ScopeOption.Required)
		{
			Option = option;
		}
	}
}