using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.TimeService
{
    public class GameSDKTimeService : ITimeService, IDisposable
    {
        private long _syncTimestamp;
        private float _localTimeAtSync;
        private DateTime _syncUtcTime;
        private IDisposable _updater;

        public async Task Initialize()
        {
            await SyncTimestamp();
            _updater = Observable.Interval(TimeSpan.FromMinutes(1), UnityTimeProvider.TimeUpdateRealtime)
                .Subscribe(OnSyncTimestamp);
        }

        public long GetCurrentUtcTime() => _syncTimestamp +
                                           (long)(Time.realtimeSinceStartup - _localTimeAtSync) *
                                           TimeSpan.TicksPerSecond;

        public DateTime GetCurrentUtcDateTime() => 
            _syncUtcTime.AddMilliseconds((Time.realtimeSinceStartup - _localTimeAtSync) * 1000);

        private async void OnSyncTimestamp(Unit _) => await SyncTimestamp();

        private async Task SyncTimestamp()
        {
            var timestamp = await GameSDK.Time.Time.GetTimestamp();
            if (timestamp <= 0) 
                timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            
            _syncTimestamp = timestamp;
            _syncUtcTime = DateTimeOffset.FromUnixTimeMilliseconds(_syncTimestamp).UtcDateTime;
            _localTimeAtSync = Time.realtimeSinceStartup;
        }

        public void Dispose()
        {
            _updater?.Dispose();
        }
    }
}