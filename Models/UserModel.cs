using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LandedMVC.Dtos;

namespace LandedMVC.Models
{
    public class UserModel
    {

		public int Id { get; set; }

        [Required]
		public string FirstName { get; set; } = string.Empty;

        [Required]
		public string LastName { get; set; } = string.Empty;

		public string? Notes { get; set; }

        public UserDto ToDto() => new() { 
            Id = Id, 
            FirstName = FirstName, 
            LastName = LastName, 
            Notes = Notes ?? string.Empty 
        };

    }
}
