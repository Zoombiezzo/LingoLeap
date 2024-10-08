using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RequirementService.Requirements.LevelCompletedRequirements;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public interface IBankItem
    {
        string Id { get; }
        string Name { get; }
        float Price { get; }
        string PromoText { get; }
        string ProductId { get; }
        IRewardInfo Reward { get; }
        bool IsRequirementLevel { get; }
        LevelNeedRequirement LevelNeedRequirement { get; }
        bool IsLimitationOnCountPurchase { get; }
        int CountPurchaseLimit { get; }
        string SpriteId { get; }
        CurrencyType Currency { get; }
        IBankItemView View { get; }
    }
}