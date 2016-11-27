using System;

namespace DemoApplication.Data.Entities
{
	public class Movie
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ImdbUrl { get; set; }
		public DateTimeOffset UpdatedOn { get; set; }
	}
}