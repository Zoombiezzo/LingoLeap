using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory.CategoryFactories
{
    public class GridCategoryFactory : IBankCategoryFactory
    {
        public ICategoryView Create(Transform parent, ICategoryView prefab, IBankCategory item)
        {
            if (prefab is not CategoryView categoryViewPrefab)
                return null;
            
            var view = Object.Instantiate(categoryViewPrefab, parent);
            
            view.Initialize(item);
            
            return view;
        }
    }
}