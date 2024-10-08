using _Client.Scripts.Infrastructure.Services.WalletService;

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement.Types.Currency
{
    public class CurrencyRewardProvider : IRewardProvider
    {
        private readonly IWalletService _walletService;

        public CurrencyRewardProvider(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public bool IsPossibleGetReward(IReward rewardInfo, out string error)
        {
            error = string.Empty;
            return true;
        }

        public bool GetReward(IReward rewardInfo, out string error)
        {
            error = string.Empty;

            if (rewardInfo is not CurrencyReward currencyReward)
            {
                error = "Wrong reward type";
                return false;
            }
            
            if (_walletService.TryAddCurrency(currencyReward.CurrencyType, currencyReward.Count, out var diff) == false)
            {
                error = $"{currencyReward.CurrencyType} is full";
                return false;
            }
            
            return true;
        }
    }
}