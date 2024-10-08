using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    public interface ILimiter
    {
        bool IsLimitOver { get; }
        List<LimitationRecord> Limitations { get; set; }
        void LimitOver();
        void LimitNotOver();
        void LimitUpdated(LimitationRecord record);
    }
}