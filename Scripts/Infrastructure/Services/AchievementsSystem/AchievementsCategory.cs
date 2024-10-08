using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [CreateAssetMenu(fileName = "AchievementsCategory", menuName = "Achievements/AchievementsCategory", order = 0)]
    [System.Serializable]
    public class AchievementsCategory : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _description;
        [SerializeField] private string _title;
        [SerializeField] private AchievementInfo[] _achievements;
        
        public string Id => _id;
        public string Description => _description;
        public string Title => _title;
        public AchievementInfo[] Achievements => _achievements;
    }
}