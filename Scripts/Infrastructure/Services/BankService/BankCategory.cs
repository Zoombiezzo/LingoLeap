using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    [CreateAssetMenu(fileName = "BankCategoryConfig", menuName = "Configs/Bank/BankCategoryConfig", order = 1)]
    public class BankCategory : ScriptableObject, IBankCategory
    {
        [FoldoutGroup("GENERAL")] [SerializeField]
        protected string _id;

        [FoldoutGroup("GENERAL")] [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _name;

        [FoldoutGroup("CATEGORIES")] [OnValueChanged("CategoriesChanged")] [SerializeField]
        protected List<BankCategory> _categories;

        [FoldoutGroup("ITEMS")] [SerializeField]
        protected List<BankItem> _items;
        
        [FoldoutGroup("VIEW")] [SerializeField]
        protected CategoryView _viewPrefab;

        public string Name => _name;
        public string Id => _id;
        public List<IBankCategory> Categories => _categories.Cast<IBankCategory>().ToList();
        public List<IBankItem> Items => _items.Cast<IBankItem>().ToList();
        public ICategoryView View => _viewPrefab;

#if UNITY_EDITOR
        private void CategoriesChanged()
        {
            Debug.Log("Categories changed");
            if (_categories == null) return;

            if (_categories.Any(el => el != null && el.GetInstanceID() == this.GetInstanceID()))
            {
                Debug.LogError("BankCategory: " + this.name + " has a reference to itself");
                _categories = _categories.Where(el => el != null && el.GetInstanceID() != this.GetInstanceID())
                    .ToList();
            }
        }
#endif
    }
}