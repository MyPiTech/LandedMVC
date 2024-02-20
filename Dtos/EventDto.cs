using TestMVC.Attributes;
using TestMVC.Models;

namespace TestMVC.Dtos
{
    [ApiRoute("/Users/{uId}/Events")]
    public class EventDto
    {
        [ApiKey]
        public int Id { get; set; }
        [ApiKey("uId")]
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public int Duration { get; set; }

        public EventModel ToModel() => new() { 
            Id = Id, 
            UserId = UserId, 
            Title = Title, 
            Location = Location, 
            Start = Start, 
            Duration = Duration 
        };
    }
}
