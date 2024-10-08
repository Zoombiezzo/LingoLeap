using System.Collections.Generic;
using _Client.Scripts.GameLoop.Screens.CloudCurtain;
using _Client.Scripts.GameLoop.Screens.CompletedLevelWindow;
using _Client.Scripts.GameLoop.Screens.MainMenu;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class TransitionFromCompletedToMainMenuState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;

        public TransitionFromCompletedToMainMenuState(IGameStateMachine stateMachine, ISceneService sceneService)
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

            await UniTask.WaitUntil(() => completedLevelWindow.IsShow() == false);

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