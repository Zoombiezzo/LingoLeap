using System;

namespace _Client.Scripts.Infrastructure.Services.AdvertisingService
{
    public interface IAdvertisingProvider : IDisposable
    {
        event Action OnInterstitialShowed;
        event Action OnInterstitialClosed;
        event Action OnInterstitialFailed;

        event Action OnRewardedShowed;
        event Action OnRewardedClosed;
        event Action OnRewardedReward;
        event Action OnRewardedFailed;
        
        void Initialize();
        void ShowInterstitial();
        void ShowRewarded();
        void ShowBanner();
        void HideBanner();
    }
}