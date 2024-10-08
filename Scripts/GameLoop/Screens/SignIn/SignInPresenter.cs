using System;
using _Client.Scripts.GameLoop.Screens.SignIn;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.RateService;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.CloudStorage;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.LocalStorage;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Settings
{
    public class SignInPresenter : IStartable, IDisposable
    {
        private SignInWindow _window;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;

        private IDisposable _disposable;

        public SignInPresenter(IAuthService authService, IStorageService storageService, IGameStateMachine stateMachine)
        {
            _authService = authService;
            _storageService = storageService;
            _gameStateMachine = stateMachine;
        }

        public void Start()
        {
            WindowsService.TryGetWindow(out _window);

            var disposableBuilder = Disposable.CreateBuilder();

            _window.ClosePanel.OnClickAsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            _window.CloseButton.OnClick.AsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            _window.SignInButton.OnClick.AsObservable().Subscribe(OnSignIn).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void OnClickClose(Unit _)
        {
            _authService.CancelSignIn(true);
            _window.Hide();
        }
        
        private async void OnSignIn(Unit _)
        {
            await _authService.SignIn();

            if (_authService.SignInType == SignInType.Account)
            {
                _window.Hide();

                var currentInfo = _storageService.CurrentStorageInfo;

                _storageService.RegisterStorage<CloudStorage>(new CloudStorage(_authService));

                var loadInfo = await _storageService.TryLoadInfo<CloudStorage>();

                if (loadInfo.Item1)
                {
                    var cloudStorageInfo = loadInfo.Item2;

                    var diffDate = new TimeSpan(Math.Abs(currentInfo.SaveTime - cloudStorageInfo.SaveTime));

                    if (diffDate.TotalSeconds > 0)
                    {
                        ReloadGame();
                        return;
                    }

                }
                else
                {
                    await _storageService.ResolveSaveByStorage<LocalStorage>();
                    _storageService.SetPreferredStorage<CloudStorage>();
                }
            }
        }

        private async void ReloadGame()
        {
            WindowsService.TryGetWindow<SignInWindow>(out var signInWindow);
            signInWindow.Show();
            
            await UniTask.WaitUntil(() => signInWindow.IsShow() == false);

            await _storageService.ResolveSaveByStorage<CloudStorage>();
            _storageService.SetPreferredStorage<CloudStorage>();
            
            _window.Hide();
            
            _gameStateMachine.Enter<ReloadSavesState>();
        }
    }
}