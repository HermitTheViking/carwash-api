namespace Domain.Events.Wash
{
    internal class WashUpdatedEvent : IEvent
    {
        public string WashId { get; set; }
        public bool Done { get; set; }
    }
}