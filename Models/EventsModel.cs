using LandedMVC.Dtos;

namespace LandedMVC.Models
{
    public class EventsModel : DataModel<UserEventDto>
    {
        public int UserId { get; set; }
    }
}
