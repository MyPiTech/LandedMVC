using TestMVC.Attributes;
using TestMVC.Models;

namespace TestMVC.Dtos
{
    [ApiRoute("/Users")]
    public class UserDto
    {
        [ApiKey]
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public UserModel ToModel() => new() { 
            Id = Id, 
            FirstName = FirstName, 
            LastName = LastName, 
            Notes = Notes 
        };
    }
}
