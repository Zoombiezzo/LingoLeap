using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    public interface ISpinSetting
    {
        CurrencyType Currency { get; }
        int Price { get; }
    }
}