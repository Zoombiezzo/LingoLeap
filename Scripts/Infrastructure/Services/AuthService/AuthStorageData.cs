using System;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AuthService
{
    [Serializable]
    public class AuthStorageData : IStorage
    {
        [SerializeField] public bool IsCanceled = false;
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }

        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}