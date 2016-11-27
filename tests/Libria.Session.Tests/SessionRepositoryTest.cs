using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Libria.Session.EF6;
using Libria.Session.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Libria.Session.Tests
{
	[TestClass]
	public class SessionRepositoryTest
	{
		[TestMethod]
		public async Task TestSessionScopeAsync()
		{
			var tmpRepoData = new List<Test>();

			var mockSet = new Mock<DbSet<Test>>();
			mockSet.Setup(m => m.Add(It.IsAny<Test>())).Returns<Test>(s =>
			{
				tmpRepoData.Add(s);
				return s;
			});
			mockSet.Setup(m => m.Remove(It.IsAny<Test>())).Returns<Test>(s =>
			{
				tmpRepoData.Remove(s);
				return s;
			});
			mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
				.Returns<object[]>(s => tmpRepoData.FirstOrDefault(r => r.Id == (int)s[0]));

			var mockContext = new Mock<TestDbContext>();
			mockContext.Setup(m => m.Set<Test>()).Returns(mockSet.Object);
			var mockFactory = new Mock<ISessionConnectionFactory>();
			var mockConnection = new Mock<DbContextSessionConnection<TestDbContext>>(mockContext.Object);
			mockFactory.Setup(m => m.CreateSessionConnection<DbContextSessionConnection<TestDbContext>>())
				.Returns(mockConnection.Object);


			var factory = new SessionScopeFactory(mockFactory.Object, null);
			var ct = CancellationToken.None;
			
			using (var scope = factory.Create())
			{
				var repository = new TestRepository(new SessionConnectionLocator());

				var test = await repository.AddAsync(new Test()
				{
					Id = 1,
					Name = "Test1"
				}, ct);

				await scope.SaveChangesAsync(ct);
			}

			Assert.IsTrue(tmpRepoData.Any());
		}
	}
	
}
