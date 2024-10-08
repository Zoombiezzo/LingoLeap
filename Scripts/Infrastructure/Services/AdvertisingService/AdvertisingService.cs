using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;

namespace _Client.Scripts.Infrastructure.Services.AdvertisingService
{
    public class AdvertisingService : IAdvertisingService
    {
        private readonly IAdvertisingProvider _advertisingProvider;
        
        public event Action OnShowedInterstitial;
        public event Action OnClosedInterstitial;
        public event Action OnFailedInterstitial;
        public event Action OnShowedRewarded;
        public event Action OnClosedRewarded;
        public event Action OnRewardRewarded;
        public event Action OnFailedRewarded;
        public bool AdsShowing => _interstitialAdsShowing || _rewardedAdsShowing;

        private List<IAdvertisingReceiver> _rewardedReceivers = new();
        private List<IAdvertisingReceiver> _interstitialReceivers = new();

        private bool _isRewarded = false;
        private bool _interstitialAdsShowing = false;
        private bool _rewardedAdsShowing = false;

        public AdvertisingService(IAdvertisingProvider advertisingProvider)
        {
            _advertisingProvider = advertisingProvider;
        }

        private void Subscribe()
        {
            _advertisingProvider.OnInterstitialShowed += OnShowedInterstitialHandler;
            _advertisingProvider.OnInterstitialClosed += OnClosedInterstitialHandler;
            _advertisingProvider.OnInterstitialFailed += OnFailedInterstitialHandler;
            
            _advertisingProvider.OnRewardedShowed += OnShowedRewardedHandler;
            _advertisingProvider.OnRewardedClosed += OnClosedRewardedHandler;
            _advertisingProvider.OnRewardedReward += OnRewardRewardedHandler;
            _advertisingProvider.OnRewardedFailed += OnFailedRewardedHandler;
        }

        private void Unsubscribe()
        {
            _advertisingProvider.OnInterstitialShowed -= OnShowedInterstitialHandler;
            _advertisingProvider.OnInterstitialClosed -= OnClosedInterstitialHandler;
            _advertisingProvider.OnInterstitialFailed -= OnFailedInterstitialHandler;
            
            _advertisingProvider.OnRewardedShowed -= OnShowedRewardedHandler;
            _advertisingProvider.OnRewardedClosed -= OnClosedRewardedHandler;
            _advertisingProvider.OnRewardedReward -= OnRewardRewardedHandler;
            _advertisingProvider.OnRewardedFailed -= OnFailedRewardedHandler;
        }

        private void OnFailedRewardedHandler()
        {
            foreach (var receiver in _rewardedReceivers)
            {
                if (_isRewarded)
                    receiver.SuccessShowed();
                else
                    receiver.FailShowed();
            }
            
            _rewardedReceivers.Clear();
            
            OnFailedRewarded?.Invoke();
            
            _isRewarded = false;
        }

        private void OnRewardRewardedHandler()
        {
            _isRewarded = true;
            OnRewardRewarded?.Invoke();
        }

        private void OnClosedRewardedHandler()
        {
            foreach (var receiver in _rewardedReceivers)
            {
                if (_isRewarded)
                    receiver.SuccessShowed();
                else
                    receiver.FailShowed();
            }
            
            _rewardedReceivers.Clear();

            _rewardedAdsShowing = false;

            AudioService.ResumeAll();
            OnClosedRewarded?.Invoke();
            
            _isRewarded = false;
        }

        private void OnShowedRewardedHandler()
        {
            foreach (var receiver in _rewardedReceivers)
            {
                receiver.AdsShowed();
            }

            _rewardedAdsShowing = true;
            
            AudioService.PauseAll();
            OnShowedRewarded?.Invoke();
        }

        private void OnClosedInterstitialHandler()
        {
            foreach (var receiver in _interstitialReceivers)
            {
                receiver.SuccessShowed();
            }
            
            _interstitialReceivers.Clear();
            
            _interstitialAdsShowing = false;

            AudioService.ResumeAll();
            OnClosedInterstitial?.Invoke();
        }

        private void OnFailedInterstitialHandler()
        {
            foreach (var receiver in _interstitialReceivers)
            {
                receiver.FailShowed();
            }
            
            _interstitialReceivers.Clear();
            
            AudioService.ResumeAll();
            OnFailedInterstitial?.Invoke();
        }

        private void OnShowedInterstitialHandler()
        {
            foreach (var receiver in _interstitialReceivers)
            {
                receiver.AdsShowed();
            }

            _interstitialAdsShowing = true;
            AudioService.PauseAll();
            OnShowedInterstitial?.Invoke();
        }

        public void Dispose()
        {
            _advertisingProvider?.Dispose();
            Unsubscribe();
        }

        public void Initialize()
        {
            _advertisingProvider.Initialize();
            Subscribe();
        }

        public void ShowInterstitial(IAdvertisingReceiver receiver)
        {
            _interstitialReceivers.Add(receiver);
            
            _advertisingProvider?.ShowInterstitial();
        }

        public void ShowRewarded(IAdvertisingReceiver receiver)
        {
            _isRewarded = false;
            _rewardedReceivers.Add(receiver);
            
            _advertisingProvider?.ShowRewarded();
        }

        public void ShowBanner()
        {
            _advertisingProvider?.ShowBanner();
        }

        public void HideBanner()
        {
            _advertisingProvider?.HideBanner();
        }
    }
}