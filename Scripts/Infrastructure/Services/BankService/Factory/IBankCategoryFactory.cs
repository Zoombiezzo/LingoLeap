using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory
{
    public interface IBankCategoryFactory
    {
        ICategoryView Create(Transform parent, ICategoryView prefab, IBankCategory item);
    }
}