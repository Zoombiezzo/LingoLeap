using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    public interface ISpinWheelService : IConfigData, IStorable
    {
        public event Action OnSpinUsed;
        
        IReadOnlyList<ISpinWheelItem> CurrentSpinItems { get; }
        bool TryGetReward(int index, out IRewardInfo rewardInfo);
        void Purchase(Action<bool> callback = null);
        bool IsPossibleSpin();
        int GetSpinLeft();
        int GetMaxSpins();
        int GetRandomIndex();
        bool TryGetCurrentSpinSetting(out ISpinSetting setting);
        TimeSpan GetTimeLeftToUpdate();
        bool CanUpdate();
        void UpdateSpins();
    }
}