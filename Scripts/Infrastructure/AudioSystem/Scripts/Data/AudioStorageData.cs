using System;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts.Data
{
    [Serializable]
    public class AudioStorageData : IAudioStorageData
    {
        [SerializeField] private float _volumeSound;
        [SerializeField] private float _volumeMusic;
        
        public float VolumeSound => _volumeSound;
        public float VolumeMusic => _volumeMusic;
        
        public int Version => 0;
        
        public IStorage ToStorage(string data)
        {
            return JsonUtility.FromJson<AudioStorageData>(data);
        }

        public string ToData(IStorable data)
        {
            var audioService = (AudioService)data;
            _volumeSound = audioService.GetVolumeInternal(AudioType.Sound);
            _volumeMusic = audioService.GetVolumeInternal(AudioType.Music);
            return JsonUtility.ToJson(this);
        }
    }

    public interface IAudioStorageData : IStorage
    {
        float VolumeSound { get; }
        float VolumeMusic { get; }
    }
}