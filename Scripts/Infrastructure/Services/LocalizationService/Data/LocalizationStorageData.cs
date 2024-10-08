using System;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService.Data
{
    [Serializable]
    public class LocalizationStorageData : ILocalizationStorageData
    {
        [SerializeField] public string _currentLocalizationCode;
        [SerializeField] public bool _userChanged;
        
        public string CurrentLocalizationCode => _currentLocalizationCode;
        public bool UserChanged => _userChanged;
        public int Version => 0;
        public IStorage ToStorage(string data)
        {
            return JsonUtility.FromJson<LocalizationStorageData>(data);
        }

        public string ToData(IStorable data)
        {
            var localizationData = (LocalizationService)data;
            _currentLocalizationCode = localizationData.CurrentLanguageCode;
            _userChanged = localizationData.UserChanged;
            return JsonUtility.ToJson(this);
        }
    }
}