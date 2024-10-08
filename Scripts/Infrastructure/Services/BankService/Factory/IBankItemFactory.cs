using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory
{
    public interface IBankItemFactory
    {
        IBankItemView Create(Transform parent, IBankItemView prefab, IBankItem item);
    }
}