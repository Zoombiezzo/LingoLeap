using System;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public interface ITutorialExecutor : IDisposable
    {
        public string Id { get; }
        public bool IsRunning { get; }
        bool Execute(ITutorialConfig config, ITutorialData data);
        void Reset();
    }
}