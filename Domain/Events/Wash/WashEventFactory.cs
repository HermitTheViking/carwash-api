using Domain.Databse.Models;

namespace Domain.Events.Wash
{
    public class WashEventFactory
    {
        public IEvent GetWashCreatedEvent(WashDbModel wash)
        {
            return new WashCreatedEvent()
            {
                UserId = wash.UserId,
                Type = wash.Type,
                StartTime = wash.StartTime,
                Duration = wash.Duration
            };
        }

        public IEvent GetWashUpdatedEvent(WashDbModel wash)
        {
            return new WashUpdatedEvent()
            {
                WashId = wash.Id,
                Done = wash.Done
            };
        }
    }
}