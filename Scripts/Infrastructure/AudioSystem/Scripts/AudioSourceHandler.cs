using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public class AudioSourceHandler : IAudioSourceHandler
    {
        private AudioSource _source;
        private AudioClip _clip;
        private AudioChannel _channel;
        private float _clipLength;

        private Coroutine _coroutineComplete;
        private IEnumerator _delegateChecker;
        private Sequence _sequenceVolumePlaying;
        private Sequence _sequenceVolumeStopped;
        private Sequence _sequenceVolumePaused;
        private Sequence _sequenceVolumeResumed;

        private AudioFlags _flags;

        private AudioSettings _settings = AudioSettings.Default;

        public bool IsPlaying => _source.isPlaying;
        public string ClipName => _source.clip.name;

        public AudioFlags Flags => _flags;

        public AudioClip Clip => _source.clip;

        public AudioSourceHandler(AudioChannel channel, AudioSource source)
        {
            _channel = channel;
            _source = source;
            _delegateChecker = CheckComplete();
        }

        internal void Play(AudioClip clip)
        {
            var time = 0f;

            if (_clip != null && clip == _clip)
            {
                time = _source.time;
            }
            
            _clip = clip;

            _clipLength = _clip != null ? _clip.length : 0f;
            
            _source.clip = _clip;
            _source.Play();
            _source.time = time;
            
            _flags |= AudioFlags.AudioRunning;
            
           // Debug.Log($"[TRACER]: Audio running {_clip?.name} state: {_flags}");
            
            if (_settings.DurationPlay > 0f)
            {
                _sequenceVolumePlaying?.Kill();
                _source.volume = 0f;
                _sequenceVolumePlaying = DOTween.Sequence()
                    .Append(_source.DOFade(_settings.Volume, _settings.DurationPlay))
                    .OnComplete(OnAudioPlaying);
            }
            
            StartCheckComplete();
            
            void OnAudioPlaying()
            {
                _flags |= AudioFlags.AudioPlaying;
                _flags &= ~AudioFlags.AudioRunning;
                
               // Debug.Log($"[TRACER]: Audio playing {_clip?.name} state: {_flags}");
            }
        }

        private void ResumeStreamingClip()
        {
            if(_clip == null)
                return;
            
            if(_clip.loadType != AudioClipLoadType.Streaming)
                return;

            var time = _source.time;
            _source.Play();
            _source.time = time;
            
           // Debug.Log($"[TRACER]: Resume streaming clip {_clip.name} time: {_source.time}");
        }

        internal void PlayOneShot(AudioClip clip)
        {
            _source.clip = clip;
            _source.PlayOneShot(clip);
            _flags |= AudioFlags.AudioPlaying;
            
            StartCheckComplete();
        }

        internal void Stop()
        {
            StopCheckComplete();

            _flags |= AudioFlags.AudioStopping;
            
           // Debug.Log($"[TRACER]: Audio stopping {_clip?.name} state: {_flags}");
            
            if (_settings.DurationStop > 0f)
            {
                _sequenceVolumeStopped?.Kill();
                _sequenceVolumeStopped = DOTween.Sequence()
                    .Append(_source.DOFade(0f, _settings.DurationStop))
                    .OnComplete(StopAnimationInternal);
                return;
            }

            StopAnimationInternal();

            void StopAnimationInternal()
            {
                _flags |= AudioFlags.AudioStopped;
                _flags &= ~AudioFlags.AudioStopping;
                
               // Debug.Log($"[TRACER]: Audio stopped {_clip?.name} state: {_flags}");

                _source.Stop();
                RemoveFromPlaying();
            }
        }

        private void BreakStopping()
        {
            _sequenceVolumeStopped?.Kill();
        }
        
        private void BreakPlaying()
        {
            _sequenceVolumePlaying?.Kill();
        }
        
        private void BreakPausing()
        {
            _sequenceVolumePaused?.Kill();
        }
        
        private void BreakResuming()
        {
            _sequenceVolumeResumed?.Kill();
        }

        private void BreakRunningOrStopping()
        {
            if (_flags.HasFlag(AudioFlags.AudioRunning))
            {
                BreakPlaying();
            }

            if (_flags.HasFlag(AudioFlags.AudioStopping))
            {
                BreakStopping();
            }
        }
        
        private void ContinueRunningOrStopping()
        {
            if (_flags.HasFlag(AudioFlags.AudioRunning))
            {
                Play(_clip);
            }

            if (_flags.HasFlag(AudioFlags.AudioStopping))
            {
                Stop();
            }
        }

        internal void Pause(bool manual = false)
        {
            if (_flags.HasFlag(AudioFlags.AudioPaused))
                return;
            
            StopCheckComplete();
            BreakRunningOrStopping();
            BreakResuming();
            
            if (manual)
                _flags |= AudioFlags.AudioManualPaused;
            
            _flags |= AudioFlags.AudioPaused;
            
           // Debug.Log($"[TRACER]: Audio pausing {_clip?.name} state: {_flags}");

            if (_settings.DurationPause > 0f && manual)
            {
                _sequenceVolumePaused?.Kill();
                _sequenceVolumePaused = DOTween.Sequence()
                    .Append(_source.DOFade(0f, _settings.DurationPause))
                    .OnComplete(PauseAnimationInternal);
                return;
            }

            PauseAnimationInternal();
            
            void PauseAnimationInternal()
            {
                _source.Pause();
                _source.volume = 0f;
                
               // Debug.Log($"[TRACER]: Audio paused {_clip?.name} state: {_flags}");
            }
        }

        internal void Unpause(bool manual = false)
        {
            if (manual == false && _flags.HasFlag(AudioFlags.AudioManualPaused))
                return;
            
            if(_flags.HasFlag(AudioFlags.AudioStopped))
                return;

            BreakPausing();
                
            _source.UnPause();
            
            if (manual)
                _flags &= ~AudioFlags.AudioManualPaused;
            
            _flags &= ~AudioFlags.AudioPaused;
            
           // Debug.Log($"[TRACER]: Audio unpausing {_clip?.name} state: {_flags}");
            
            if (_settings.DurationResume > 0f && manual)
            {
                _sequenceVolumeResumed?.Kill();
                _sequenceVolumeResumed = DOTween.Sequence()
                    .Append(_source.DOFade(_settings.Volume, _settings.DurationResume)
                        .OnComplete(OnUnpaused));
                return;
            }

            OnUnpaused();


            void OnUnpaused()
            {
                if (manual == false)
                {
                    ResumeStreamingClip();
                }

                StartCheckComplete();
                _source.volume = _settings.Volume;
                
               // Debug.Log($"[TRACER]: Audio unpaused {_clip?.name} state: {_flags}");

                ContinueRunningOrStopping();
            }
        }

        internal void Clear()
        {
           // Debug.Log($"[TRACER]: Audio clear {_clip?.name} state: {_flags}");

            _source.clip = null;
            _flags = AudioFlags.None;
        }

        private void StartCheckComplete()
        {
            if (_flags.HasFlag(AudioFlags.AudioStopped))
                return;
            
            StopCheckComplete();
            _coroutineComplete = _channel.StartCoroutine(CheckComplete());
        }

        private void StopCheckComplete()
        {
            if (_coroutineComplete != null)
            {
                _channel.StopCoroutine(_coroutineComplete);
                _coroutineComplete = null;
            }
        }

        private IEnumerator CheckComplete()
        {
            var prevTime = GetClipRemainingTime();
            var waiter = new WaitForSeconds(0.1f);
            
            while (_source != null)
            {
                if (_source.isPlaying || _source.loop)
                {
                   /*Debug.Log($"[TRACER]: Playing clip {ClipName} isPlaying: {_source.isPlaying} loop: {_source.loop} source volume: {_source.volume} audio service volume music: {AudioService.GetVolume(AudioType.Music)} audio service volume sound: {AudioService.GetVolume(AudioType.Sound)}" +
                              $"is muted: {_source.mute} time: {_source.time} clip loaded: {_clip.loadState}");*/
                    yield return null;
                    continue;
                }
                
               // Debug.Log($"[TRACER]: Clip {ClipName} remaining time: {GetClipRemainingTime()}");

                if (GetClipRemainingTime() > 0f)
                {
                    yield return waiter;
                    
                    var currentTime = GetClipRemainingTime();

                   // Debug.Log($"[TRACER]: Clip {ClipName} remaining time: {currentTime}");

                    if (prevTime - currentTime <= 0f)
                    {
                        
                       // Debug.Log($"[TRACER]: Clip {ClipName} prevTime: {prevTime} currentTime: {currentTime}");
                        break;
                    }
                    
                    prevTime = currentTime;
                    yield return null;
                    continue;
                }
                
               // Debug.Log($"[TRACER]: Clip {ClipName} finished");
                
                break;
            }
            
            StopCheckComplete();
            RemoveFromPlaying();
        }
        
        private bool IsReversePitch() {
            return _source.pitch < 0f;
        }
        
        private float GetClipRemainingTime() {
            float remainingTime = (_clipLength - _source.time) / _source.pitch;
            return IsReversePitch() ?
                (_clipLength + remainingTime) : remainingTime;
        }

        public void SetSettings(AudioSettings settings)
        {
            _source.pitch = settings.Pitch;
            _source.volume = settings.Volume;
            _source.loop = settings.Loop;
            _settings = settings;
        }

        public void SetDefaultSettings()
        {
            SetSettings(AudioSettings.Default);
        }

        private void RemoveFromPlaying()
        {
            _channel.RemoveFromPlaying(this);
        }
    }
}