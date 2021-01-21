using System;

namespace Domain.Events
{
    public class EventInfo
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string EventType { get; set; }

        public IEvent Event { get; set; }
    }
}