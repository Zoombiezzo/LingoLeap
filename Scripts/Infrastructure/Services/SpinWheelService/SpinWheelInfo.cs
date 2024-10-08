using System.Collections.Generic;
using _Client.Scripts.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    [CreateAssetMenu(fileName = "SpinWheelInfo", menuName = "SpinWheel/SpinWheelInfo", order = 0)]
    public class SpinWheelInfo : ScriptableObject
    {
        [SerializeField]
        [FoldoutGroup("Настройка времени обновления наград")]
        [HideLabel]
        private DateTimeInfo _timeUpdateSpin;
        
        [SerializeField]
        [FoldoutGroup("Настройка спинов")]
        private List<SpinSetting> _spinSettings;
        
        [SerializeField]
        [FoldoutGroup("Настройка регионов наград")]
        private List<SpinWheelRegionInfo> _spinWheelItems;
        
        public List<SpinWheelRegionInfo> Regions => _spinWheelItems;
        public List<SpinSetting> SpinSettings => _spinSettings;
        public DateTimeInfo TimeUpdateSpin => _timeUpdateSpin;
    }
}