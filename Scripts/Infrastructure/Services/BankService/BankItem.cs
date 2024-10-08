using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Helpers;
using _Client.Scripts.Infrastructure.Services.BankService.Views;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RequirementService;
using _Client.Scripts.Infrastructure.Services.RequirementService.Requirements.LevelCompletedRequirements;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    [CreateAssetMenu(fileName = "BankItem", menuName = "Configs/Bank/BankItem", order = 1)]
    public class BankItem : ScriptableObject, IBankItem
    {
        [SerializeField] protected string _id;
        [SerializeField] protected RewardInfo _reward;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _name;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _promoText;
        [SerializeField] protected CurrencyType _currency;
        [SerializeField] [ShowIf("@_currency == CurrencyType.Real")]
        protected string _productId;
        [SerializeField] protected float _price;
        [SerializeField] [ValueDropdown("SpritesDropdown")]
        private string _spriteId;
        [SerializeField] protected BankItemView _viewPrefab;

        [FoldoutGroup("Требования для покупки:")]
        [SerializeField] [OnValueChanged("IsRequirementLevelChanged")] 
        protected bool _isRequirementLevel;
        
        [FoldoutGroup("Требования для покупки:")]
        [SerializeField] [ShowIf("_isRequirementLevel")] 
        protected LevelNeedRequirement _levelNeedRequirement;
        
        [FoldoutGroup("Ограничениия для покупки")]
        [FoldoutGroup("Ограничениия для покупки/Ограничение по количеству")]
        [LabelText("Ограничение по количеству:")]
        [SerializeField]
        protected bool _isLimitationOnCountPurchase; 
        
        [FoldoutGroup("Ограничениия для покупки")]
        [FoldoutGroup("Ограничениия для покупки/Ограничение по количеству")]
        [LabelText("Количество:")] [ShowIf("_isLimitationOnCountPurchase")]
        [SerializeField]
        protected int _countPurchaseLimit;
        
        public string Id => _id;
        public string PromoText => _promoText;
        public string ProductId => _productId;
        public IRewardInfo Reward => _reward;
        public string Name => _name;
        public bool IsLimitationOnCountPurchase => _isLimitationOnCountPurchase;
        public int CountPurchaseLimit => _countPurchaseLimit;
        public bool IsRequirementLevel => _isRequirementLevel;
        public LevelNeedRequirement LevelNeedRequirement => _levelNeedRequirement;
        public CurrencyType Currency => _currency;
        public float Price => _price;
        public string SpriteId => _spriteId;
        public IBankItemView View => _viewPrefab;
        
#if UNITY_EDITOR

        private void IsRequirementLevelChanged()
        {
            if (_isRequirementLevel && _levelNeedRequirement == null)
            {
                _levelNeedRequirement = new LevelNeedRequirement();
            }
            else if (!_isRequirementLevel && _levelNeedRequirement != null)
            {
                _levelNeedRequirement = null;
            }
        }
        
        private IEnumerable<string> SpritesDropdown()
        {
            var spritesIds = new List<string>();
            var configs = AssetHelper<SpritesPreset>.GetAsset("Assets");
            foreach (var spritesPreset in configs)
            {
                foreach (var sprites in spritesPreset.SpritePresets)
                {
                    spritesIds.Add(spritesPreset.GetIdSprite(sprites.Id));
                }
            }

            return spritesIds;
        }
        
        // [FoldoutGroup("Настройки")]
        // [Button(ButtonSizes.Medium, Name = "Добавить ограничение")]
        // private void AddRequirement()
        // {
        //     GenericMenu menu = new();
        //
        //     foreach (var sampleType in TypesHelper<IRequirement>.GetTypesChild())
        //     {
        //         var sample = (IRequirement)Activator.CreateInstance(sampleType);
        //         menu.AddItem(new GUIContent(sample.RequirementName), false, AppendRequirement, sample);
        //     }
        //
        //     menu.ShowAsContext();
        //
        //     void AppendRequirement( object typeRequirement )
        //     {
        //         if ( typeRequirement is not IRequirement requirement )
        //         {
        //             Debugger.LogError( $"[BankItem]: Wrong requirement. Current: {typeRequirement.GetType().Name}!" );
        //             return;
        //         }
        //
        //         _requirements.Add( requirement );
        //         
        //         EditorUtility.SetDirty(this);
        //         AssetDatabase.SaveAssets();
        //         AssetDatabase.Refresh();
        //     }
        // }
#endif
    }
}