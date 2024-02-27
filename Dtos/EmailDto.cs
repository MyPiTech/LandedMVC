using LandedMVC.Attributes;
using System.Text.Json.Serialization;

namespace TestApi.Dtos
{
	[ApiRoute("/Email")]
	public class EmailDto
    {
        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }


    }  
}
