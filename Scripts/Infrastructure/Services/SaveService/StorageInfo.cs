using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    [System.Serializable]
    public class StorageInfo
    {
        [SerializeField] private int _version;
        [SerializeField] private long _saveTime;
        
        public int Version
        {
            get => _version;
            internal set => _version = value;
        }

        public long SaveTime
        {
            get => _saveTime;
            internal set => _saveTime = value;
        }

        public StorageInfo(int version, long saveTime = 0)
        {
            _version = version;
            _saveTime = saveTime;
        }
    }
}