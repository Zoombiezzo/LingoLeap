using System.Collections;
using _Client.Scripts.Infrastructure.Services.AdvertisingService;
using UnityEngine;
using VContainer;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public class AudioServiceBehaviour : MonoBehaviour
    {
        private IAdvertisingService _advertisingService;

        private bool _hasFocus;
        private bool _isPaused;
        private Coroutine _coroutineChecker;

        [Inject]
        public void Construct(IAdvertisingService advertisingService)
        {
            _advertisingService = advertisingService;
        }
        
        private void Start()
        {
            _coroutineChecker = StartCoroutine(CheckAudioStatus());
        }

        private IEnumerator CheckAudioStatus()
        {
            var waiter = new WaitForSeconds(1f);
            while (true)
            {
                yield return waiter;

                var isFocus = Application.isFocused;
                if (isFocus == _hasFocus) continue;
                
                _hasFocus = isFocus;
                TryChangePauseStatusAudio();
            }
        }
        
        private void OnEnable()
        {
            Application.focusChanged += OnApplicationFocus;
        }

        private void OnDisable()
        {
            Application.focusChanged -= OnApplicationFocus;
        }

        private void TryChangePauseStatusAudio()
        {
            if (_advertisingService != null && _advertisingService.AdsShowing)
            {
                AudioService.PauseAll();
                return;
            }
            
            if (_hasFocus && !_isPaused)
            {
                AudioService.ResumeAll();
                return;
            }
            
            AudioService.PauseAll();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _isPaused = pauseStatus;
            TryChangePauseStatusAudio();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _hasFocus = hasFocus;
            TryChangePauseStatusAudio();
        }
    }
}