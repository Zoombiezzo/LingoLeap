using System;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    [Serializable]
    public class CountLimitationRecord : LimitationRecord
    {
        [JsonProperty] private int _count;
        
        [JsonIgnore] public int Count => _count;
        
        public override event Action<LimitationRecord> OnValueChanged;
        public override event Action<LimitationRecord> OnCompleted;
        
        public CountLimitationRecord()
        {
            
        }
        
        public CountLimitationRecord(int count)
        {
            _count = count;
        }

        public void DecreaseValue(int count)
        {
            if (_count == 0) return;

            count = Mathf.Max(0, count);
            
            _count -= count;

            if (_count < 0)
                _count = 0;

            OnValueChanged?.Invoke(this);
            
            if (IsComplete())
            {
                OnCompleted?.Invoke(this);
            }
        }

        public override bool IsComplete()
        {
            return _count <= 0;
        }

        public override void Reset()
        {
            _count = ((CountLimitation)_limitation).Count;
            OnValueChanged?.Invoke(this);
        }
    }
}