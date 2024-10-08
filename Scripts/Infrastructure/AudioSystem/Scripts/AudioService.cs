using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts.Data;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    public sealed class AudioService : IStorable, IConfigData
    {
        private const string AssetPath = "Audio";
        
        private static AudioService s_instance = null;

        private List<IAudioChannel> _allChannels = new(3);
        private Dictionary<string, IAudioChannel> _channelsIds = new(3);
        private Dictionary<AudioType, List<IAudioChannel>> _channelsTypes = new(3);
        private Dictionary<string, AudioClip> _clipsIds = new(25);

        private IStorageService _storage;
        private IAssetProvider _assetProvider;
        public event Action OnChanged;
        public event Action OnAudioChanged;

        public static AudioService Instance => s_instance;
        
        public static IReadOnlyList<IAudioChannel> AllChannels => s_instance?._allChannels;

        public static void RegisterStorage(IStorageService storageService)
        {
            s_instance?.RegisterStorageInternal(storageService);
        }
        
        public static void RegisterAssetProvider(IAssetProvider assetProvider)
        {
            s_instance?.RegisterAssetProviderInternal(assetProvider);
        }

        public static void Register(IAudioChannel audioChannel)
        {
            s_instance?.RegisterInternal(audioChannel);
        }

        public static void Unregister(IAudioChannel audioChannel)
        {
            s_instance?.UnregisterInternal(audioChannel);
        }

        public static void RegisterBundle(AudioBundle bundle)
        {
            s_instance?.RegisterBundleInternal(bundle);
        }

        public static void RegisterClip(string id, AudioClip clip)
        {
            s_instance?.RegisterClipInternal(id, clip);
        }
        
        public static void SetVolume(AudioType type, float volume)
        {
            s_instance?.SetVolumeInternal(type, volume);
        }
        
        public static float GetVolume(AudioType type)
        {
            if (s_instance == null) return 0f;
            
            return s_instance.GetVolumeInternal(type);
        }

        public static IAudioSourceHandler Play(AudioType type, string id, AudioSettings settings = default)
        {
            return s_instance?.PlayInternal(type, id, settings);
        }

        public static IAudioSourceHandler PlayOneShot(AudioType type, string id, AudioSettings settings = default)
        {
            return s_instance?.PlayOneShotInternal(type, id, settings);
        }

        public static void Stop(string id)
        {
            s_instance?.StopInternal(id);
        }

        public static void Pause(string id)
        {
            s_instance?.PauseInternal(id);
        }

        public static void PauseAll()
        {
            s_instance?.PauseAllInternal();
        }

        public static void Resume(string id)
        {
            s_instance?.ResumeInternal(id);
        }
        
        public static void ResumeAll()
        {
            s_instance?.ResumeAllInternal();
        }

        internal void SetVolumeInternal(AudioType type, float volume)
        {
            if (_channelsTypes.TryGetValue(type, out var channels) == false)
            {
                return;
            }

            foreach (var channel in channels)
            {
                channel.SetVolume(volume);
            }
        }

        internal float GetVolumeInternal(AudioType type)
        {
            if (_channelsTypes.TryGetValue(type, out var channels) == false)
            {
                return 0f;
            }

            return channels[0].GetVolume();
        }

        private void RegisterStorageInternal(IStorageService storageService)
        {
            _storage = storageService;
        }
        
        private void RegisterAssetProviderInternal(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        private void PauseInternal(string id)
        {
            if (_clipsIds.TryGetValue(id, out var clip) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Pause clip [" + id + "] failed! Clip is not registered!");
#endif
                return;
            }

            foreach (var channel in _allChannels)
            {
                if (channel.IsPlaying(clip) == false) continue;

                channel.Pause(clip);
            }
            
            OnAudioChanged?.Invoke();
        }

        private void PauseAllInternal()
        {
            foreach (var channel in _allChannels)
            {
                if (channel.IsPlayingAny() == false) continue;
                
                channel.PauseAll();
            }
            
            OnAudioChanged?.Invoke();
        }

        private void ResumeInternal(string id)
        {
            if (_clipsIds.TryGetValue(id, out var clip) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Resume clip [" + id + "] failed! Clip is not registered!");
#endif
                return;
            }

            foreach (var channel in _allChannels)
            {
                if (channel.IsPlaying(clip) == false) continue;

                channel.Unpause(clip);
            }
            
            OnAudioChanged?.Invoke();
        }

        private void ResumeAllInternal()
        {
            foreach (var channel in _allChannels)
            {
                if (channel.IsPlayingAny() == false) continue;
                
                channel.UnpauseAll();
            }
            
            OnAudioChanged?.Invoke();
        }

        private void StopInternal(string id)
        {
            if (_clipsIds.TryGetValue(id, out var clip) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Stop clip [" + id + "] failed! Clip is not registered!");
#endif
                return;
            }

            foreach (var channel in _allChannels)
            {
                if (channel.IsPlaying(clip) == false) continue;

                channel.Stop(clip);
            }
            
            OnAudioChanged?.Invoke();
        }

        private IAudioSourceHandler PlayOneShotInternal(AudioType type, string id, AudioSettings settings)
        {
            if (_clipsIds.TryGetValue(id, out var clip) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Play one shot clip [" + id + "] failed! Clip is not registered!");
#endif
                return null;
            }

            if (TryGetFreeChannel(type, out var usedChannel) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Play one shot clip [" + id + "] failed! No free channel!");
#endif
                return null;
            }
            
            if (settings.Equals(default) == false && settings.CheckDuplicate)
            {
                if (CheckPlayingClip(type, clip))
                    return null;
            }

            var handler = usedChannel.PlayOneShot(clip, settings);

            OnAudioChanged?.Invoke();

            return handler;
        }

        private IAudioSourceHandler PlayInternal(AudioType type, string id, AudioSettings settings)
        {
            if (_clipsIds.TryGetValue(id, out var clip) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Play clip [" + id + "] failed! Clip is not registered!");
#endif
                return null;
            }

            if (TryGetFreeChannel(type, out var usedChannel) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Play clip [" + id + "] failed! No free channel!");
#endif
                return null;
            }

            if (settings.Equals(default) == false && settings.CheckDuplicate)
            {
                if (CheckPlayingClip(type, clip))
                    return null;
            }

            var handler = usedChannel.Play(clip, settings);
            
            OnAudioChanged?.Invoke();
            return handler;
        }

        private bool TryGetFreeChannel(AudioType type, out IAudioChannel usedChannel)
        {
            usedChannel = null;

            if (_channelsTypes.TryGetValue(type, out var channelList) == false)
            {
                return false;
            }

            foreach (var channel in channelList)
            {
                if (channel.FreeSourceCount > 0)
                {
                    usedChannel = channel;
                    return true;
                }
            }

            if (usedChannel == null && channelList.Count > 0)
            {
                usedChannel = channelList[0];
                return true;
            }

            return false;
        }

        private bool CheckPlayingClip(AudioType type, AudioClip clip)
        {
            if (_channelsTypes.TryGetValue(type, out var channelList) == false)
            {
                return false;
            }

            foreach (var channel in channelList)
            {
                if (channel.IsPlaying(clip)) return true;
            }

            return false;
        }

        private void RegisterInternal(IAudioChannel audioChannel)
        {
            if (s_instance == null) return;
            if (_channelsIds.ContainsKey(audioChannel.Id))
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Register channel [" + audioChannel.Id +
                          "] failed! Channel is already registered!");
#endif
                return;
            }

            _channelsIds.Add(audioChannel.Id, audioChannel);

            if (_channelsTypes.TryGetValue(audioChannel.AudioType, out var channelList) == false)
            {
                channelList = new List<IAudioChannel>();
                _channelsTypes.Add(audioChannel.AudioType, channelList);
            }

            channelList.Add(audioChannel);
            _allChannels.Add(audioChannel);

#if UNITY_EDITOR
            Debug.Log("[AudioService]: Register channel [" + audioChannel.Id + "] success!");
#endif

            audioChannel.Initialize();
        }

        private void UnregisterInternal(IAudioChannel audioChannel)
        {
            if (_channelsIds.ContainsKey(audioChannel.Id) == false)
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Unregister channel [" + audioChannel.Id +
                          "] failed! Channel is not registered!");
#endif
                return;
            }

            _channelsIds.Remove(audioChannel.Id);

            if (_channelsTypes.TryGetValue(audioChannel.AudioType, out var channelList))
            {
                channelList.Remove(audioChannel);
            }

            _allChannels.Remove(audioChannel);

#if UNITY_EDITOR
            Debug.Log("[AudioService]: Unregister channel [" + audioChannel.Id + "] success!");
#endif

            audioChannel.Uninitialize();
        }

        private void RegisterBundleInternal(AudioBundle bundle)
        {
            foreach (var info in bundle.Infos)
            {
                RegisterClipInternal(info.Id, info.Clip);
            }
        }

        private void RegisterClipInternal(string id, AudioClip clip)
        {
            if (_clipsIds.ContainsKey(id))
            {
#if UNITY_EDITOR
                Debug.Log("[AudioService]: Register clip [" + id + "] failed! Clip is already registered!");
#endif
                return;
            }

            _clipsIds.Add(id, clip);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            s_instance = new AudioService();
        }

        public void Load(IStorage data)
        {
            var storageData = (AudioStorageData)data;
            SetVolumeInternal(AudioType.Music, storageData.VolumeMusic);
            SetVolumeInternal(AudioType.Sound, storageData.VolumeSound);
            
            OnChanged?.Invoke();
        }

        public string ToStorage()
        {
            var storage = _storage.Get<AudioService>();
            return storage.Storage.ToData(this);
        }

        public async Task LoadData()
        {
            var bundles = await _assetProvider.LoadAll<AudioBundle>(AssetPath);
            
            if(bundles == null)
                return;
            
            foreach (var bundle in bundles)
            {
                RegisterBundleInternal(bundle);
            }
        }
    }
}