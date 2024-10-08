using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    public interface IPurchaseDataFactory
    {
        IPurchaseData Create(ISpinSetting spinSetting);
    }
}