using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public interface IBankPurchaseDataFactory
    {
        IPurchaseData Create(IBankItem item);
    }
}