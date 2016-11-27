using System.Data.Entity.ModelConfiguration;
using DemoApplication.Data.Entities;

namespace DemoApplication.Data.Configurations
{
	public class MovieMapping : EntityTypeConfiguration<Movie>
	{
		public MovieMapping()
		{
			HasKey(e => e.Id);
		}
	}
}