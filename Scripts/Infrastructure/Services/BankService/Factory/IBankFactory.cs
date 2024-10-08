using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory
{
    public interface IBankFactory : IService
    {
        ICategoryView Create(Transform parent, ICategoryView prefab, IBankCategory category);
        IBankItemView Create(Transform parent, IBankItemView prefab, IBankItem item);
    }
}