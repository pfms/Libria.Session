using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;

namespace DemoApplication.Data
{
	public class MovieDbContext : DbContext
	{
		public MovieDbContext(string connectionString)
			: base(connectionString)
		{
			Database.Log = c => Debug.WriteLine(c);
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
			modelBuilder.Configurations.AddFromAssembly(typeof (MovieDbContext).Assembly);
		}
	}
}