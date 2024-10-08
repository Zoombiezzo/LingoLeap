using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Screens.CloudCurtain;
using _Client.Scripts.GameLoop.Screens.CompletedLevelWindow;
using _Client.Scripts.GameLoop.Screens.LoadingCurtain;
using _Client.Scripts.Infrastructure.Bootstrap;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class LoadNextLevelGameState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;

        public LoadNextLevelGameState(IGameStateMachine stateMachine, ISceneService sceneService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
        }

        public async void Enter()
        {
            WindowsService.TryGetWindow<CloudCurtainWindow>(out var window);
            window.Show();
            await UniTask.WaitUntil(() => window.IsShow());
            
            WindowsService.TryGetWindow<CompletedLevelWindow>(out var completedLevelWindow);
            completedLevelWindow.Hide();

            await _sceneService.UnloadScenesFromPreset(ScenePresetsKeys.GameRestart, immediately: true);

            await UniTask.WaitUntil(() => completedLevelWindow.IsShow() == false);
            
            _gameStateMachine.Enter<LoadGameState>();
        }

        public void Exit()
        {
            
        }
    }
}