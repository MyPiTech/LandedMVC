using System.ComponentModel.DataAnnotations;
using LandedMVC.Dtos;

namespace LandedMVC.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

		[StringLength(40)]
		public string Title { get; set; } = string.Empty;

		[StringLength(40)]
		public string? Location { get; set; } = string.Empty;

        [Required]
        public DateTime? Start { get; set; }

        [Required]
        [Range(10, 100)]
        public int? Duration { get; set; }

        public EventDto ToDto() => new() { 
            Duration = Duration ?? default, 
            UserId = UserId, 
            Title = Title, 
            Id = Id, 
            Location = Location ?? string.Empty, 
            Start = Start ?? default 
        };

		public UserEventDto ToUserEventDto() => new()
		{
			Duration = Duration ?? default,
			UserId = UserId,
			Title = Title,
			Id = Id,
			Location = Location ?? string.Empty,
			Start = Start ?? default
		};
	}
}
