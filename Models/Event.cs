using System.Text.RegularExpressions;

namespace EventManager.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string Game { get; set; }
        public int MaxParticipants { get; set; }
        public bool IsLive { get; set; }
        public ICollection<Match>? Matches { get; set; }
        public ICollection<EventRegistration>? Registrations { get; set; }
    }

}
