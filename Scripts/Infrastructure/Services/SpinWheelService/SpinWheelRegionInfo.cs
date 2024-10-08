using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    [Serializable]
    [CreateAssetMenu(fileName = "SpinWheelInfoRegion", menuName = "SpinWheel/Spin Wheel Info Region", order = 0)]
    public class SpinWheelRegionInfo : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private List<SpinWheelItemInfo> _spinWheelItems;
        [SerializeField] private SpinWheelItemInfo _baseItem;

        public string Id => _id;
        public List<SpinWheelItemInfo> SpinWheelItems => _spinWheelItems;
        public SpinWheelItemInfo BaseItem => _baseItem;
    }
}