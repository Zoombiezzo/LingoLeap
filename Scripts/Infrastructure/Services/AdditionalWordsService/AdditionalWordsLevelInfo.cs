using System;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AdditionalWordsService
{
    [Serializable]
    public class AdditionalWordsLevelInfo : IAdditionalWordsLevelInfo
    {
        [SerializeField] [LabelText("Требуемое кол-во слов")]
        private int _requiredWordsCount;
        [SerializeField] [LabelText("Награда")]
        private RewardInfo _reward;
        
        public int RequiredWordsCount => _requiredWordsCount;
        public RewardInfo Reward => _reward;
    }
}