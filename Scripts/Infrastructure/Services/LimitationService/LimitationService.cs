using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LimitationService.Data;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    public class LimitationService : ILimitationService
    {
        private Dictionary<string, List<LimitationUpdater>> _limitationUpdaters;
        private Dictionary<string, Dictionary<Type, LimitationUpdater>> _typeLimitationUpdaters;
        
        private Dictionary<string, Dictionary<string, List<LimitationRecord>>> _limitationRecordsContext;
        private Dictionary<string, Dictionary<Type, List<LimitationRecord>>> _limitationRecordsTypesUpdaters;
        private Dictionary<ILimitation, LimitationRecord> _limitationRecord;
        private Dictionary<ILimiter, List<LimitationRecord>> _limiterRecords;
        private Dictionary<LimitationRecord, List<ILimiter>> _recordLimiters;

        private List<LimitationRecord> _limitationRecordsSavable;
        private HashSet<LimitationRecord> _limitationRecordsSavableHashSet;
        
        private readonly IStorageService _storageService;

        public event Action OnChanged;
        
        internal List<LimitationRecord> LimitationRecordsSavable => _limitationRecordsSavable;
        
        public LimitationService(IStorageService storageService)
        {
            _storageService = storageService;
            
            _limitationUpdaters = new Dictionary<string, List<LimitationUpdater>>(10);
            _typeLimitationUpdaters = new Dictionary<string, Dictionary<Type, LimitationUpdater>>(10);

            _limitationRecordsContext = new Dictionary<string, Dictionary<string, List<LimitationRecord>>>(10);
            _limitationRecordsTypesUpdaters = new Dictionary<string, Dictionary<Type, List<LimitationRecord>>>(10);
            _limitationRecord = new Dictionary<ILimitation, LimitationRecord>(10);
            _limiterRecords = new Dictionary<ILimiter, List<LimitationRecord>>(10);
            _recordLimiters = new Dictionary<LimitationRecord, List<ILimiter>>(10);
            _limitationRecordsSavableHashSet = new HashSet<LimitationRecord>(10);
            
            _limitationRecordsSavable = new List<LimitationRecord>(10);
        }
        
        public void RegisterUpdater(string context, LimitationUpdater updater)
        {
            if (_limitationUpdaters.TryGetValue(context, out var limitationUpdaters) == false)
            {
                limitationUpdaters = new List<LimitationUpdater>();
                _limitationUpdaters.Add(context, limitationUpdaters);
            }
            
            limitationUpdaters.Add(updater);
            
            if (_typeLimitationUpdaters.TryGetValue(context, out var typeLimitationUpdaters) == false)
            {
                typeLimitationUpdaters = new Dictionary<Type, LimitationUpdater>();
                _typeLimitationUpdaters.Add(context, typeLimitationUpdaters);
            }
            
            typeLimitationUpdaters.Add(updater.GetType(), updater);

            if (_limitationRecordsTypesUpdaters.TryGetValue(context, out var limitationsForUpdate))
            {
                if (limitationsForUpdate.TryGetValue(updater.GetType(), out var limitations))
                {
                    foreach (var limitation in limitations)
                    {
                        updater.AddLimitation(limitation);
                    }
                }
            }
        }

        public void UnregisterUpdater(string context, LimitationUpdater updater)
        {
            if (_limitationUpdaters.TryGetValue(context, out var limitationUpdaters))
            {
                limitationUpdaters.Remove(updater);
            }
            
            if (_typeLimitationUpdaters.TryGetValue(context, out var typeLimitationUpdaters))
            {
                typeLimitationUpdaters.Remove(updater.GetType());
            }
        }

        public void RegisterLimitation<T>(string context, string id, ILimitation limitation, ILimiter limiter, bool isSavable = true)
            where T : LimitationUpdater
        {
            if (_limitationRecord.TryGetValue(limitation, out var record) == false)
            {
                if (_limitationRecordsContext.TryGetValue(context, out var limitationRecords) == false)
                {
                    limitationRecords = new Dictionary<string, List<LimitationRecord>>();
                    _limitationRecordsContext.Add(context, limitationRecords);
                }

                if (limitationRecords.TryGetValue(id, out var limitationList) == false)
                {
                    limitationList = new List<LimitationRecord>();
                    limitationRecords.Add(id, limitationList);
                }

                foreach (var limitationRecord in limitationList)
                {
                    if (limitationRecord.UpdaterType == typeof(T).Name)
                    {
                        record = limitationRecord;
                        break;
                    }
                }
                
                if (record == null)
                {
                    record = limitation.CreateRecord();
                    record.SetContext(context)
                        .SetId(id)
                        .SetUpdaterType(typeof(T).Name);

                    limitationList.Add(record);

                    if (isSavable)
                    {
                        _limitationRecordsSavable.Add(record);
                        _limitationRecordsSavableHashSet.Add(record);
                    }
                }

                if (record.Limitation == null)
                {
                    record.RegisterLimitation(limitation);
                }

                if (_limitationRecordsTypesUpdaters.TryGetValue(context, out var limitationTypesUpdaters) == false)
                {
                    limitationTypesUpdaters = new Dictionary<Type, List<LimitationRecord>>();
                    _limitationRecordsTypesUpdaters.Add(context, limitationTypesUpdaters);
                }

                if (limitationTypesUpdaters.TryGetValue(typeof(T), out var limitations) == false)
                {
                    limitations = new List<LimitationRecord>();
                    limitationTypesUpdaters.Add(typeof(T), limitations);
                }

                if (limitations.Contains(record) == false)
                {
                    limitations.Add(record);
                    _limitationRecord.Add(limitation, record);
                    
                    if(_typeLimitationUpdaters.TryGetValue(context, out var typeLimitationUpdaters))
                    {
                        if (typeLimitationUpdaters.TryGetValue(typeof(T), out var typeLimitationUpdater))
                        {
                            typeLimitationUpdater.AddLimitation(record);
                        }
                    }

                    record.OnValueChanged += OnLimitationValueChanged;
                    record.OnCompleted += OnLimitationCompleted;
                }
            }
            
            if(_limiterRecords.TryGetValue(limiter, out var limiterRecords) == false)
            {
                limiterRecords = new List<LimitationRecord>();
                _limiterRecords.Add(limiter, limiterRecords);
            }

            if (limiterRecords.Contains(record) == false)
            {
                limiterRecords.Add(record);
            }

            if (_recordLimiters.TryGetValue(record, out var limiters) == false)
            {
                limiters = new List<ILimiter>();
                _recordLimiters.Add(record, limiters);
            }

            if (limiters.Contains(limiter) == false)
            {
                limiters.Add(limiter);
            }

            if (limiter.Limitations == null)
            {
                limiter.Limitations = new List<LimitationRecord>();
            }

            if (limiter.Limitations.Contains(record) == false)
            {
                limiter.Limitations.Add(record);
            }

            bool isLimitOver = false;
            foreach (var limiterRecord in limiterRecords)
            {
                if (limiterRecord.IsComplete())
                {
                    isLimitOver = true;
                    break;
                }
            }
            
            limiter.LimitUpdated(record);
            
            if (isLimitOver)
            {
                limiter.LimitOver();
            }
            else
            {
                limiter.LimitNotOver();
            }
            
        }

        public void UnregisterLimitation<T>(string context, string id, ILimitation limitation, ILimiter limiter)
            where T : LimitationUpdater
        {
            if (_limitationRecord.TryGetValue(limitation, out var limitationRecord) == false)
            {
                return;
            }

            if (_recordLimiters.TryGetValue(limitationRecord, out var limiters))
            {
                limiters.Remove(limiter);
                limiter.Limitations?.Remove(limitationRecord);
            }
            
            if (_limitationRecordsTypesUpdaters.TryGetValue(context, out var limitationTypesUpdaters))
            {
                if (limitationTypesUpdaters.TryGetValue(typeof(T), out var limitations))
                {
                    limitations.Remove(limitationRecord);

                    if (_typeLimitationUpdaters.TryGetValue(context, out var typeLimitationUpdaters))
                    {
                        if (typeLimitationUpdaters.TryGetValue(typeof(T), out var updater))
                        {
                            updater.RemoveLimitation(limitationRecord);
                        }
                    }
                }
            }

            if (_limitationRecordsContext.TryGetValue(context, out var limitationRecords))
            {
                if (limitationRecords.TryGetValue(id, out var limitationList))
                {
                    limitationList.Remove(limitationRecord);
                }
            }
            
            if(_limiterRecords.TryGetValue(limiter, out var limiterRecords))
            {
                _limiterRecords.Remove(limiter);
            }
            
            if (limiters != null && limiters.Count > 0)
            {
                return;
            }

            _recordLimiters.Remove(limitationRecord);
            _limitationRecord.Remove(limitation);
            
            if (_limitationRecordsSavableHashSet.Contains(limitationRecord))
            {
                _limitationRecordsSavable.Remove(limitationRecord);
                _limitationRecordsSavableHashSet.Remove(limitationRecord);
            }

            limitationRecord.OnValueChanged -= OnLimitationValueChanged;
            limitationRecord.OnCompleted -= OnLimitationCompleted;
        }
        
        private void OnLimitationCompleted(LimitationRecord limitationRecord)
        {
            if (_recordLimiters.TryGetValue(limitationRecord, out var limiters) == false)
            {
                return;
            }

            foreach (var limiter in limiters)
            {
                if(limiter.IsLimitOver)
                    continue;
                
                limiter.LimitOver();
            }
        }

        private async void OnLimitationValueChanged(LimitationRecord limitationRecord)
        {
            if (_recordLimiters.TryGetValue(limitationRecord, out var limiters) == false)
            {
                return;
            }

            foreach (var limiter in limiters)
            {
                limiter.LimitUpdated(limitationRecord);
                
                if(_limiterRecords.TryGetValue(limiter, out var limiterRecords))
                {
                    foreach (var record in limiterRecords)
                    {
                        if(record.IsComplete() == limiter.IsLimitOver)
                            continue;

                        if (record.IsComplete())
                        {
                            limiter.LimitOver();
                        }
                        else
                        {
                            limiter.LimitNotOver();
                        }
                    }
                }
            }

            if (_limitationRecordsSavableHashSet.Contains(limitationRecord))
            {
               await _storageService.Save<ILimitationService>();
            }
        }

        private void RegisterLoadedLimitationRecord(LimitationRecord record)
        {
            var context = record.Context;
            var id = record.Id;

            if (_limitationRecordsContext.TryGetValue(context, out var limitationRecords) == false)
            {
                limitationRecords = new Dictionary<string, List<LimitationRecord>>();
                _limitationRecordsContext.Add(context, limitationRecords);
            }

            if (limitationRecords.TryGetValue(id, out var limitationList) == false)
            {
                limitationList = new List<LimitationRecord>();
                limitationRecords.Add(id, limitationList);
            }

            limitationList.Add(record);
            _limitationRecordsSavable.Add(record);
            _limitationRecordsSavableHashSet.Add(record);
        }

        public void Load(IStorage data)
        {
            var storageData = (LimitationStorageData)data;

            if (storageData.LimitationRecords != null)
            {
                foreach (var record in storageData.LimitationRecords)
                {
                    RegisterLoadedLimitationRecord(record);
                }
            }
        }

        public string ToStorage()
        {
            var storage = _storageService.Get<ILimitationService>();
            return storage.Storage.ToData(this);
        }
    }
}