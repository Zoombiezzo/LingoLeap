using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    public abstract class LimitationUpdater
    {
        protected List<LimitationRecord> _limitation = new List<LimitationRecord>();
        
        internal void AddLimitation(LimitationRecord record)
        {
            _limitation.Add(record);
        }

        internal void RemoveLimitation(LimitationRecord limitationRecord)
        {
            _limitation.Remove(limitationRecord);
        }
    }
}