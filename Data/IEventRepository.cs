using EventManager.Models;

namespace EventManager.Data
{
    public interface IEventRepository
    {
        //Events
        public IQueryable<Event> GetEventsQuery();
        public Task<Event> GetEventByIdAsync(int id);
    }
}
