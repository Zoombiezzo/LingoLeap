using _Client.Scripts.GameLoop.Screens.CloudCurtain;
using _Client.Scripts.GameLoop.Screens.LoadingCurtain;
using _Client.Scripts.GameLoop.Screens.MainMenu;
using _Client.Scripts.Infrastructure.Bootstrap;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class LoadGameState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;

        public LoadGameState(IGameStateMachine stateMachine, ISceneService sceneService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
        }

        public async void Enter()
        {
            WindowsService.TryGetWindow<CloudCurtainWindow>(out var window);
            window.Show();
            await UniTask.WaitUntil(() => window.IsShow());

            WindowsService.TryGetWindow(out MainMenuWindow mainMenuWindow);
            mainMenuWindow.Hide();
            
            await UniTask.WaitUntil(() => mainMenuWindow.IsShow() == false);
            
            await _sceneService.LoadScene(SceneNames.Game);
            await _sceneService.LoadScenesFromPreset(ScenePresetsKeys.Game);
            _sceneService.SetActiveScene(SceneNames.Game);
            
            window.Hide();
            
            await UniTask.WaitUntil(() => window.IsShow() == false);
            
            _gameStateMachine.Enter<GameState>();
        }

        public void Exit()
        {
            
        }
    }
}