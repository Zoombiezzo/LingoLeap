using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.WalletService
{
    public interface IWalletProvider
    {
        bool TryAddCurrency(CurrencyType currencyType, int count, out int diff);
        bool TryRemoveCurrency(CurrencyType currencyType, int count, out int diff);
        bool IsCurrencyEnough(CurrencyType currencyType, int count, out int diff);
    }
}