using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public class AudioChannel : MonoBehaviour, IAudioChannel
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioType _audioType;
        [SerializeField] private AudioMixerGroup _audioMixer;
        [SerializeField] private List<AudioSource> _sources = new List<AudioSource>();

        public string Id => _id;
        public int FreeSourceCount => _freeSources.Count;
        public IReadOnlyList<AudioSourceHandler> PlayingSources => _playingSourcesList;
        public AudioType AudioType => _audioType;
        public AudioMixerGroup AudioMixer => _audioMixer;
        public IReadOnlyList<AudioSource> Sources => _sources;

        private Stack<AudioSourceHandler> _freeSources = new(10);
        private HashSet<AudioSourceHandler> _playingSources = new(10);
        private List<AudioSourceHandler> _playingSourcesList = new(10);

        private Dictionary<AudioClip, List<AudioSourceHandler>> _playingClips =
            new Dictionary<AudioClip, List<AudioSourceHandler>>(10);

        private void Awake()
        {
            AudioService.Register(this);
        }

        public void Initialize()
        {
            for (var index = 0; index < _sources.Count; index++)
            {
                var source = _sources[index];
                source.outputAudioMixerGroup = _audioMixer;
                source.playOnAwake = false;

                CreateSourceHandler(source);
            }
        }

        public void Uninitialize()
        {
            foreach (var source in _sources)
            {
                source.Stop();
            }
        }

        public void SetVolume(float volume)
        {
            _audioMixer.audioMixer.SetFloat("Volume", Mathf.Log10(Mathf.Clamp(volume, Mathf.Epsilon, 1f)) * 20f);
        }
        
        public float GetVolume()
        {
            _audioMixer.audioMixer.GetFloat("Volume", out float value);
            return Mathf.Clamp01(Mathf.Pow(10f, value / 20f));
        }

        public IAudioSourceHandler Play(AudioClip clip, AudioSettings settings)
        {
            if (_freeSources.Count == 0)
            {
                AddNewAudioSource();
            }

            var source = _freeSources.Pop();
            _playingSources.Add(source);
            _playingSourcesList.Add(source);

            if (_playingClips.TryGetValue(clip, out var handlers))
            {
                handlers.Add(source);
            }
            else
            {
                _playingClips.Add(clip, new List<AudioSourceHandler>());
                _playingClips[clip].Add(source);
            }

            if (settings.Equals(default))
            {
                source.SetDefaultSettings();
            }
            else
            {
                source.SetSettings(settings);
            }
            
            source.Play(clip);
            return source;
        }

        public IAudioSourceHandler PlayOneShot(AudioClip clip, AudioSettings settings)
        {
            if (_freeSources.Count == 0)
            {
                AddNewAudioSource();
            }

            var source = _freeSources.Pop();
            _playingSources.Add(source);
            _playingSourcesList.Add(source);

            if (_playingClips.TryGetValue(clip, out var handlers))
            {
                handlers.Add(source);
            }
            else
            {
                _playingClips.Add(clip, new List<AudioSourceHandler>());
                _playingClips[clip].Add(source);
            }
            
            if (settings.Equals(default))
            {
                source.SetDefaultSettings();
            }
            else
            {
                source.SetSettings(settings);
            }

            source.PlayOneShot(clip);
            return source;
        }

        public void Stop(AudioClip clip)
        {
            if (_playingClips.TryGetValue(clip, out var handlers) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[AudioService]: Stop clip [" + clip + "] failed! Clip is not played!");
#endif
                return;
            }

            for (var index = 0; index < handlers.Count; index++)
            {
                var handler = handlers[index];
                handler.Stop();
            }
        }

        public void Pause(AudioClip clip)
        {
            if (_playingClips.TryGetValue(clip, out var handlers) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[AudioService]: Pause clip [" + clip + "] failed! Clip is not played!");
#endif
                return;
            }

            for (var index = 0; index < handlers.Count; index++)
            {
                var handler = handlers[index];
                handler.Pause(true);
            }
        }

        public void PauseAll()
        {
#if UNITY_EDITOR
            Debug.Log($"[AudioService]: All clips paused in {_id} source!");
#endif
            
            foreach (var (_, handlers) in _playingClips)
            {
                foreach (var handler in handlers)
                {
                    handler.Pause();
                }
            }
        }

        public void Unpause(AudioClip clip)
        {
            if (_playingClips.TryGetValue(clip, out var handlers) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[AudioService]: Unpause clip [" + clip + "] failed! Clip is not played!");
#endif
                return;
            }

            for (var index = 0; index < handlers.Count; index++)
            {
                var handler = handlers[index];
                handler.Unpause(true);
            }
        }

        public void UnpauseAll()
        {
#if UNITY_EDITOR
            Debug.Log($"[AudioService]: All clips resumed in {_id} source!");
#endif
            
            foreach (var (_, handlers) in _playingClips)
            {
                foreach (var handler in handlers)
                {
                    handler.Unpause();
                }
            }
        }

        public bool IsPlaying(AudioClip clip)
        {
            if (_playingClips.TryGetValue(clip, out var handlers))
            {
                return handlers.Count > 0;
            }

            return false;
        }
        
        public bool IsPlayingAny()
        {
            return _playingClips.Count > 0;
        }

        private AudioSourceHandler CreateSourceHandler(AudioSource source)
        {
            var handler = new AudioSourceHandler(this, source);
            _freeSources.Push(handler);
            return handler;
        }

        private void AddNewAudioSource()
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            if (_sources.Count > 0)
            {
                var copySource = _sources[0];
                newSource.outputAudioMixerGroup = copySource.outputAudioMixerGroup;
                newSource.volume = copySource.volume;
                newSource.pitch = copySource.pitch;
                newSource.spatialBlend = copySource.spatialBlend;
                newSource.minDistance = copySource.minDistance;
                newSource.maxDistance = copySource.maxDistance;
                newSource.rolloffMode = copySource.rolloffMode;
                newSource.dopplerLevel = copySource.dopplerLevel;
                newSource.spread = copySource.spread;
                newSource.priority = copySource.priority;
                newSource.time = copySource.time;
            }

            _sources.Add(newSource);
            CreateSourceHandler(newSource);
        }

        internal void RemoveFromPlaying(AudioSourceHandler audioSourceHandler)
        {
            _playingSources.Remove(audioSourceHandler);
            _playingSourcesList.Remove(audioSourceHandler);
            _freeSources.Push(audioSourceHandler);

            var clip = audioSourceHandler.Clip;
            if (_playingClips.TryGetValue(clip, out var handlers))
            {
                handlers.Remove(audioSourceHandler);
            }

            audioSourceHandler.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateLinks();
        }

        [Button]
        private void UpdateLinks()
        {
            _sources.Clear();
            foreach (var source in GetComponents<AudioSource>())
            {
                _sources.Add(source);
                source.outputAudioMixerGroup = _audioMixer;
                source.playOnAwake = false;
            }
        }
#endif
    }
}