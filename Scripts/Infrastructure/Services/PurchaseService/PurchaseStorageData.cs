using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    [Serializable]
    public class PurchaseStorageData : IPurchaseStorageData
    {
        public List<PurchaseProduct> _pendingProducts = new();
        public int Version => 0;

        public IReadOnlyList<PurchaseProduct> PendingProducts => _pendingProducts;

        public IStorage ToStorage(string data)
        {
            return JsonUtility.FromJson<PurchaseStorageData>(data);
        }

        public string ToData(IStorable data)
        {
            var purchaseService = (PurchaseService)data;
            _pendingProducts.Clear();
            _pendingProducts.AddRange(purchaseService.PendingProducts);
            return JsonUtility.ToJson(this);
        }
    }
}