using System;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.RateService
{
    public interface IRateService : IService, IStorable
    {
        bool Rated { get; }
        bool RatedShowed { get; }
        event Action OnRatedSuccess;
        event Action OnRatedFailed;
        void Rate();
    }
}