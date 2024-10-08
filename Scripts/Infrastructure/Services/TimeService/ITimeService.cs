using System;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.TimeService
{
    public interface ITimeService : IService
    {
        Task Initialize();
        long GetCurrentUtcTime();
        DateTime GetCurrentUtcDateTime();
    }
}