using System;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    [Serializable]
    public class MapStorage : IStorage
    {
        public int Version => 0;
        [SerializeField] public string CurrentSelectedLocationId;
        [SerializeField] public int CurrentOpenedLocationIndex;
        [SerializeField] public int ProgressCounter;

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }
        
        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}