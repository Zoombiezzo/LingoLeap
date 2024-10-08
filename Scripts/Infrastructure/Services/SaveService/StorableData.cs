using System;

namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    [Serializable]
    public class StorableData<TStorable> : IStorableData where TStorable : IStorable
    {
        public IStorable Data { get; private set; }
        public IStorage Storage => _storage;
        public string DataString;
        public int Version;

        [NonSerialized] private IStorage _storage;
        
        public StorableData(TStorable storableData, IStorage storage)
        {
            Data = storableData;
            Version = storage.Version;
            _storage = storage;
        }

        public void Loaded(IStorableData data)
        {
            var storageData = (StorableData<TStorable>)data;
            Version = storageData.Version;
            DataString = storageData.DataString;
            
            if(_storage == null)
                return;
            

            if(string.IsNullOrEmpty(DataString))
                return;
            
            _storage = _storage.ToStorage(storageData.DataString);
            Data.Load(_storage);
        }
        
        public void Created(IStorableData data)
        {
            var storageData = (StorableData<TStorable>)data;
            Version = storageData.Version;
            DataString = storageData.DataString;
            
            if(_storage == null)
                return;
            
            _storage = _storage.ToStorage(storageData.DataString);
            Data.Load(_storage);
        }
    }
}