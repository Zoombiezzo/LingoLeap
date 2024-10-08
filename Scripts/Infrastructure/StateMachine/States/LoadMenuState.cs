using _Client.Scripts.GameLoop.Screens.LoadingCurtain;
using _Client.Scripts.GameLoop.Screens.MainMenu;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class LoadMenuState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;

        public LoadMenuState(IGameStateMachine stateMachine, ISceneService sceneService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
        }

        public async void Enter()
        {
            WindowsService.TryGetWindow<LoadingCurtainWindow>(out var window);
            window.Show();
            
            await _sceneService.LoadScene(SceneNames.MainMenu, LoadSceneMode.Additive, progress => window.SetProgress(progress));
            await _sceneService.LoadScenesFromPreset(ScenePresetsKeys.Main,
                (sceneId, progress) => window.SetProgress(progress));
            
            _sceneService.SetActiveScene(SceneNames.MainMenu);

            WindowsService.TryGetWindow(out MainMenuWindow mainMenuWindow);
            mainMenuWindow.Show();
            
            await UniTask.WaitUntil(() => mainMenuWindow.IsShow());

            window.Hide();
            _gameStateMachine.Enter<MenuState>();
        }

        public void Exit()
        {
            
        }
    }
}