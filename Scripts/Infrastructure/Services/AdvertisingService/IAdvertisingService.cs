using System;

namespace _Client.Scripts.Infrastructure.Services.AdvertisingService
{
    public interface IAdvertisingService : IService, IDisposable
    {
        public event Action OnShowedInterstitial;
        public event Action OnClosedInterstitial;
        public event Action OnFailedInterstitial;
        public event Action OnShowedRewarded;
        public event Action OnClosedRewarded;
        public event Action OnRewardRewarded;
        public event Action OnFailedRewarded;
        public bool AdsShowing { get; }
        public void Initialize();
        public void ShowInterstitial(IAdvertisingReceiver receiver);
        public void ShowRewarded(IAdvertisingReceiver receiver);
        public void ShowBanner();
        public void HideBanner();
    }
}