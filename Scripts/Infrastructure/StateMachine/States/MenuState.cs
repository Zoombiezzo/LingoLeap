using _Client.Scripts.GameLoop.Screens.LoadingCurtain;
using _Client.Scripts.GameLoop.Screens.MainMenu;
using _Client.Scripts.GameLoop.Screens.SignIn;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.RateService;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class MenuState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;
        private readonly MainMenuPresenter _mainMenuPresenter;
        private readonly IAuthService _authService;
        private readonly IRateService _rateService;

        public MenuState(IGameStateMachine stateMachine, ISceneService sceneService, IAuthService authService, IRateService rateService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
            _authService = authService;
            _rateService = rateService;
        }

        public async void Enter()
        {
            WindowsService.TryGetWindow<MainMenuWindow>(out var menu);
            menu.Show();
            
            //if (_tutorialService.IsAnyTutorialRunning() == false)
            //{
                if (_authService.SignInType != SignInType.Account && _authService.IsCanceled == false)
                {
                    WindowsService.Show<SignInWindow>();
                }

                if (_authService.SignInType == SignInType.Account && _rateService.RatedShowed == false)
                {
                    _rateService.Rate();
                }
            //}

            await GameSDK.Core.GameApp.GameReady();
        }

        public void Exit()
        {
            
        }
    }
}