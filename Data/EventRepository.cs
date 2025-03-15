using EventManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Data
{
    public class EventRepository : IEventRepository
    {
        private ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Event> GetEventsQuery()
        {
            return _context.Events;
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            var eventItem = await _context.Events.Where(eventItem => eventItem.EventID == id).SingleOrDefaultAsync();

            if (eventItem == null)
            {
                throw new Exception($"Event with ID {id} not found");
            }

            return eventItem;
        }
    }
}
