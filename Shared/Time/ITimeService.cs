using System;

namespace Utility.Time
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}
