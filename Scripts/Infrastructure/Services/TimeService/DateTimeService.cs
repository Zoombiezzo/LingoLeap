using System;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.TimeService
{
    public class DateTimeService : ITimeService
    {
        public Task Initialize() => Task.CompletedTask;

        public long GetCurrentUtcTime()
        {
            return DateTime.UtcNow.Ticks;
        }

        public DateTime GetCurrentUtcDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}