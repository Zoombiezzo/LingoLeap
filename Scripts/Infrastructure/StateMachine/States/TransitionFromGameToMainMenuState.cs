using System.Collections.Generic;
using _Client.Scripts.GameLoop.Screens.CloudCurtain;
using _Client.Scripts.GameLoop.Screens.MainMenu;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class TransitionFromGameToMainMenuState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;

        public TransitionFromGameToMainMenuState(IGameStateMachine stateMachine, ISceneService sceneService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
        }

        public async void Enter()
        {
            WindowsService.TryGetWindow<CloudCurtainWindow>(out var window);
            window.Show();
            await UniTask.WaitUntil(() => window.IsShow());

            await _sceneService.UnloadScene(SceneNames.Game);
            await _sceneService.UnloadScenesFromPreset(ScenePresetsKeys.Game, dontUnloadScenesFromPresets: new List<string>(){ScenePresetsKeys.Main});
            await _sceneService.LoadScene(SceneNames.MainMenu);
            await _sceneService.LoadScenesFromPreset(ScenePresetsKeys.Main);
            
            WindowsService.TryGetWindow(out MainMenuWindow mainMenuWindow);
            mainMenuWindow.Show();
            
            await UniTask.WaitUntil(() => mainMenuWindow.IsShow());
            
            window.Hide();
            
            await UniTask.WaitUntil(() => window.IsShow() == false);

            _gameStateMachine.Enter<MenuState>();
        }

        public void Exit()
        {
            
        }
    }
}