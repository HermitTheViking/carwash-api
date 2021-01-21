using System.Collections.Generic;

namespace Domain.Events
{
    public interface IEventStore
    {
        void AddEvents(params IEvent[] events);
        void AddEvents(IEnumerable<IEvent> events);

        IEnumerable<EventInfo> GetEvents();
    }
}