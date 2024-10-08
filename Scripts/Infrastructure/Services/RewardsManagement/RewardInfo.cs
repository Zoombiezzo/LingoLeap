using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement
{
    [Serializable]
    public class RewardInfo : IRewardInfo
    {
        [FoldoutGroup("Награды")] [SerializeReference] [HideReferenceObjectPicker] [DisableContextMenu]
        private List<IReward> _rewards = new List<IReward>();

        public List<IReward> Rewards => _rewards;

#if UNITY_EDITOR
        [FoldoutGroup("Настройки")]
        [Button(ButtonSizes.Medium, Name = "Добавить награду")]
        private void ChangeType()
        {
            GenericMenu menu = new();

            foreach (var sampleType in TypesHelper<IReward>.GetTypesChild())
            {
                var sample = (IReward)Activator.CreateInstance(sampleType);
                menu.AddItem(new GUIContent(sample.TypeName), false, AddReward, sampleType);
            }

            menu.ShowAsContext();

            void AddReward(object reward)
            {
                if (reward is not Type typePool)
                {
                    Debug.LogError($"[ExperienceRewardInfo]: Wrong type. Current: {reward.GetType().Name}!");
                    return;
                }

                var typeRewardItem = Activator.CreateInstance(typePool);
                if (typeRewardItem is not IReward)
                {
                    Debug.LogError(
                        $"[ExperienceRewardInfo]: Wrong reward type instance. Current: {typeRewardItem.GetType().Name}!");
                    return;
                }

                _rewards.Add((IReward)typeRewardItem);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif
    }
}