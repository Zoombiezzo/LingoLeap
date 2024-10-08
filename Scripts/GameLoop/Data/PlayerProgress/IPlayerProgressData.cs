using System;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.SaveService;
using R3;

namespace _Client.Scripts.GameLoop.Data.PlayerProgress
{
    public interface IPlayerProgressData : IStorable, IConfigData, IDisposable
    {
        ReadOnlyReactiveProperty<int> Soft { get; }
        ReadOnlyReactiveProperty<int> BoosterSelectChar { get; }
        ReadOnlyReactiveProperty<int> BoosterSelectWord { get; }
        ReadOnlyReactiveProperty<int> MindScore { get; }
        void AddMindScore(int value);
        void ChangeSoft(int value);
        void ChangeBoosterSelectChar(int value);
        void ChangeBoosterSelectWord(int value);
    }
}