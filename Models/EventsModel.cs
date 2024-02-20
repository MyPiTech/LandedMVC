using TestMVC.Dtos;

namespace TestMVC.Models
{
    public class EventsModel : DataModel<EventDto>
    {
        public int UserId { get; set; }
    }
}
