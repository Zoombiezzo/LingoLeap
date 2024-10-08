using System.Collections.Generic;
using _Client.Scripts.GameLoop.Screens.CompletedLevelWindow;
using _Client.Scripts.GameLoop.Screens.WordsLevel;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class CompletedLevelState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;

        public CompletedLevelState(IGameStateMachine stateMachine, ISceneService sceneService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
        }

        public async void Enter()
        {
            await UniTask.Delay(3000);

            WindowsService.TryGetWindow<WordsLevelWindow>(out var wordsLevelWindow);
            wordsLevelWindow.Hide();

            await UniTask.WaitUntil(() => wordsLevelWindow.IsShow() == false);

            WindowsService.TryGetWindow<CompletedLevelWindow>(out var window);
            window.Show();

            await UniTask.WaitUntil(() => window.IsShow());

            await _sceneService.UnloadScenesFromPreset(ScenePresetsKeys.GameRestart, immediately: true);
        }

        public void Exit()
        {
            
        }
    }
}