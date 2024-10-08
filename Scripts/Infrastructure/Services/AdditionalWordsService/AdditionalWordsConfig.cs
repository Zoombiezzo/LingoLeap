using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AdditionalWordsService
{
    [CreateAssetMenu(fileName = "AdditionalWordsConfig", menuName = "Configs/AdditionalWords/AdditionalWordsConfig")]
    public class AdditionalWordsConfig : ScriptableObject
    {
        [SerializeField] [LabelText("Уровни")] 
        private List<AdditionalWordsLevelInfo> _levels;
        [SerializeField] [LabelText("Базовый уровень")]
        private AdditionalWordsLevelInfo _baseLevel;
        
        public List<AdditionalWordsLevelInfo> Levels => _levels;
        public AdditionalWordsLevelInfo BaseLevel => _baseLevel;
    }
}