using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    [CreateAssetMenu(fileName = "BankConfig", menuName = "Configs/Bank/BankConfig", order = 1)]
    public class BankConfig : ScriptableObject
    {
        [FoldoutGroup("GENERAL")][SerializeField] 
        private string _id;
        [FoldoutGroup("CATEGORIES")][SerializeField] 
        private List<BankCategory> _categories;
        public string Id => _id;
        public List<BankCategory> Categories => _categories;
    }
}