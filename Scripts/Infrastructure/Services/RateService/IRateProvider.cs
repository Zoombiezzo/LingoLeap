using System;

namespace _Client.Scripts.Infrastructure.Services.RateService
{
    public interface IRateProvider
    {
        event Action OnRatedSuccess;
        event Action OnRatedFailed;
        void Rate();
    }
}