using System.ComponentModel.DataAnnotations;
using LandedMVC.Dtos;

namespace LandedMVC.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        public string? Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Start is required.")]
        [Display(Name = "Event Start")]
        public DateTime? Start { get; set; }

        [Required(ErrorMessage = "Event Duration is required.")]
        [Range(1, 100, ErrorMessage = "Enter event duration between 1 to 100.")]
        [Display(Name = "Event Duration")]
        public int? Duration { get; set; }

        public EventDto ToDto() => new() { 
            Duration = Duration ?? default, 
            UserId = UserId, 
            Title = Title, 
            Id = Id, 
            Location = Location ?? string.Empty, 
            Start = Start ?? default 
        };    
    }
}
