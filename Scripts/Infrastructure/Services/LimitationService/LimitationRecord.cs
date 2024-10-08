using System;
using Newtonsoft.Json;

namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    [Serializable]
    public abstract class LimitationRecord
    {
        [JsonProperty] protected string _context;
        [JsonProperty] protected string _id;
        [JsonProperty] protected string _updaterType;
        
        [JsonIgnore] public string Context => _context;
        [JsonIgnore] public string Id => _id;
        [JsonIgnore] public string UpdaterType => _updaterType;
        [JsonIgnore] public ILimitation Limitation => _limitation;

        public abstract event Action<LimitationRecord> OnValueChanged;
        public abstract event Action<LimitationRecord> OnCompleted;
        
        [JsonIgnore] protected ILimitation _limitation;

        internal LimitationRecord RegisterLimitation(ILimitation limitation)
        {
            _limitation = limitation;
            return this;
        }
        
        internal LimitationRecord SetContext(string context)
        {
            _context = context;
            return this;
        }

        internal LimitationRecord SetId(string id)
        {
            _id = id;
            return this;
        }

        internal LimitationRecord SetUpdaterType(string updaterType)
        {
            _updaterType = updaterType;
            return this;
        }

        public virtual void Reset()
        {
            
        }

        public virtual bool IsComplete()
        {
            return false;
        }
    }
}