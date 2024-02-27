using LandedMVC.Dtos;
using System.ComponentModel.DataAnnotations;
using TestApi.Dtos;

namespace LandedMVC.Models
{
	public class EmailModel
	{
		[Required]
		public string? Name { get; set; }
		[Required]
		public string? Message { get; set; }

		public EmailDto ToDto() => new()
		{
			Subject = $"Landed MVC - From {Name}",
			Email = string.Empty,
			Name = Name,
			Message = Message
		};
	}
}
