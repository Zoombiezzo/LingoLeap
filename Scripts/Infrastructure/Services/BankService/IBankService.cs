using System;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public interface IBankService : IConfigData, IStorable
    {
        event Action<IBankItem> OnPurchased; 
        BankConfig GetBankConfig(string id);
        BankItem GetItemConfig(string id);
        void Purchase(string id, Action<BankItem, bool> callback = null);
        Task Consume(PurchaseProduct purchaseProduct, Action<BankItem, bool> callback = null);
    }
}