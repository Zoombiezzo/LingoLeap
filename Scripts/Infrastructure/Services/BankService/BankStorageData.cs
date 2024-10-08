using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    [Serializable]
    public class BankStorageData : IStorage
    {
        public int Version => 0;
        [SerializeField] public List<PurchasedItem> PurchasedItems = new();
        
        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }
        
        public string ToData(IStorable data) => JsonUtility.ToJson(this);

    }
}
