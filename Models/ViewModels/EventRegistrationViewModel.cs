using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManager.Models.ViewModels
{
    public class EventRegistrationViewModel
    {
        public EventRegistration EventRegistration { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<SelectListItem> Events { get; set; }
    }

}
