using System;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    [Serializable]
    public class SpinSetting : ISpinSetting
    {
        [SerializeField] [LabelText("Валюта:")]
        protected CurrencyType _currency;
        [SerializeField] [LabelText("Цена:")]
        protected int _price;
        
        public CurrencyType Currency => _currency;
        public int Price => _price;
    }
}