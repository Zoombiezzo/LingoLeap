using System;
using GameSDK.Advertisement;

namespace _Client.Scripts.Infrastructure.Services.AdvertisingService
{
    public class GameSDKAdvertisingProvider : IAdvertisingProvider
    {
        public event Action OnInterstitialShowed;
        public event Action OnInterstitialClosed;
        public event Action OnInterstitialFailed;

        public event Action OnRewardedShowed;
        public event Action OnRewardedClosed;
        public event Action OnRewardedReward;
        public event Action OnRewardedFailed;

        public GameSDKAdvertisingProvider()
        {
        }

        public async void Initialize()
        {
            await Ads.Initialize();
            Subscribe();
        }

        public void ShowInterstitial()
        {
            Ads.Interstitial.Show();
        }

        public void ShowRewarded()
        {
            Ads.Rewarded.Show();
        }

        public void ShowBanner()
        {
            Ads.Banner.Show();
        }

        public void HideBanner()
        {
            Ads.Banner.Hide();
        }

        private void Subscribe()
        {
            Ads.Interstitial.OnShowed += OnShowedInterstitialHandler;
            Ads.Interstitial.OnClosed += OnClosedInterstitialHandler;
            Ads.Interstitial.OnShowFailed += OnShowFailedInterstitialHandler;
            Ads.Interstitial.OnError += OnErrorInterstitialHandler;
            Ads.Interstitial.OnClicked += OnClickedInterstitialHandler;
           
            Ads.Rewarded.OnShowed += OnShowedRewardedHandler;
            Ads.Rewarded.OnClosed += OnClosedRewardedHandler;
            Ads.Rewarded.OnRewarded += OnRewardRewardedHandler;
            Ads.Rewarded.OnError += OnErrorRewardedHandler;
            Ads.Rewarded.OnClicked += OnClickedRewardedHandler;
        }

        private void Unsubscribe()
        {
            Ads.Interstitial.OnShowed -= OnShowedInterstitialHandler;
            Ads.Interstitial.OnClosed -= OnClosedInterstitialHandler;
            Ads.Interstitial.OnShowFailed -= OnShowFailedInterstitialHandler;
            Ads.Interstitial.OnError -= OnErrorInterstitialHandler;
            Ads.Interstitial.OnClicked -= OnClickedInterstitialHandler;
            
            Ads.Rewarded.OnShowed -= OnShowedRewardedHandler;
            Ads.Rewarded.OnClosed -= OnClosedRewardedHandler;
            Ads.Rewarded.OnRewarded -= OnRewardRewardedHandler;
            Ads.Rewarded.OnError -= OnErrorRewardedHandler;
            Ads.Rewarded.OnClicked -= OnClickedRewardedHandler;
        }

        private void OnClickedRewardedHandler()
        {
            
        }

        private void OnErrorRewardedHandler()
        {
            OnRewardedFailed?.Invoke();
        }

        private void OnRewardRewardedHandler()
        {
            OnRewardedReward?.Invoke();
        }

        private void OnClosedRewardedHandler()
        {
            OnRewardedClosed?.Invoke();
        }

        private void OnShowedRewardedHandler()
        {
            OnRewardedShowed?.Invoke();
        }

        private void OnClickedInterstitialHandler()
        {
            
        }

        private void OnErrorInterstitialHandler()
        {
            OnInterstitialFailed?.Invoke();
        }

        private void OnShowFailedInterstitialHandler()
        {
            OnInterstitialFailed?.Invoke();
        }

        private void OnClosedInterstitialHandler()
        {
            OnInterstitialClosed?.Invoke();
        }

        private void OnShowedInterstitialHandler()
        {
            OnInterstitialShowed?.Invoke();
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}