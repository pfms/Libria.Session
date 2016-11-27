using System;

namespace DemoApplication.Services.Models
{
	public class MovieModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ImdbUrl { get; set; }
		public DateTimeOffset UpdatedOn { get; set; }
	}
}