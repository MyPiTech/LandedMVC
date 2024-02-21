using LandedMVC.Dtos;

namespace LandedMVC.Models
{
    public class EventsModel : DataModel<EventDto>
    {
        public int UserId { get; set; }
    }
}
