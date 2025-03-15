using EventManager.Models;

namespace EventManager.Models
{
    public class EventRegistration
    {
        public int RegistrationID { get; set; }

        public string UserID { get; set; } // FK to IdentityUser
        public AppUser User { get; set; } // Navigation property

        public int EventID { get; set; } // FK to Event
        public Event Event { get; set; } // Navigation property
    }

}
