namespace _Client.Scripts.Infrastructure.Services.AdvertisingService
{
    public interface IAdvertisingReceiver
    {
        void AdsShowed();
        void SuccessShowed();
        void FailShowed();
    }
}