using System;

namespace Domain.Time
{
    public class TimeService : ITimeService, ITimeServiceInitializer
    {
        private TimeZoneInfo _timeZoneInfo;

        public DateTime Now
        {
            get
            {
                if (_timeZoneInfo != null) { return TimeZoneInfo.ConvertTime(UtcNow, TimeZoneInfo.Utc, _timeZoneInfo); }

                return DateTime.Now;
            }
        }

        public DateTime Today => DateTime.Today;

        public DateTime UtcNow => DateTime.UtcNow;

        public void SetTimeZone(string timeZoneId)
        {
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}
