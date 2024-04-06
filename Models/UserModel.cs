using System.ComponentModel.DataAnnotations;
using LandedMVC.Dtos;

namespace LandedMVC.Models
{
    public class UserModel : ApiModel
    {

		public int Id { get; set; }

        [Required]
		[StringLength(20)]
		public string FirstName { get; set; } = string.Empty;

        [Required]
		[StringLength(20)]
		public string LastName { get; set; } = string.Empty;

		[StringLength(500)]
		public string? Notes { get; set; }

        public UserDto ToDto() => new() { 
            Id = Id, 
            FirstName = FirstName, 
            LastName = LastName, 
            Notes = Notes ?? string.Empty 
        };

    }
}
