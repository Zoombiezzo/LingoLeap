using System;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public interface IBankItemView
    {
        IBankItem Item { get; }
        event Action<IBankItemView> OnBuy;
        void Initialize(IBankItem item);
    }
}