using System;

namespace Domain.Events.Wash
{
    public class WashCreatedEvent : IEvent
    {
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public int Type { get; set; }
    }
}