using System;

namespace Domain.Time
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}
