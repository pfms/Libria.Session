using Libria.Session.EF6;
using Libria.Session.Interfaces;

namespace Libria.Session.Tests
{
	public class TestRepository : MockEFRepository<Test, TestDbContext>
	{
		public TestRepository(ISessionConnectionLocator sessionConnectionLocator) 
			: base(sessionConnectionLocator)
		{
		}
	}
}