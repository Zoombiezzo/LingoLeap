using System;
using GameSDK.GameFeedback;

namespace _Client.Scripts.Infrastructure.Services.RateService
{
    public class GameSDKRateProvider : IRateProvider
    {
        public event Action OnRatedSuccess;
        public event Action OnRatedFailed;

        public async void Rate()
        {
            var canReview = await Feedback.CanReview();

            if (canReview.Item1 == false)
            {
                OnRatedFailed?.Invoke();
                return;
            }

            var result = await Feedback.RequestReview();

            if (result.Item1)
            {
                OnRatedSuccess?.Invoke();
            }
            else
            {
                OnRatedFailed?.Invoke();
            }
        }
    }
}