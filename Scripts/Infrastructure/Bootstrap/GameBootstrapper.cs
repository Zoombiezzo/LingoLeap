using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.Bootstrap
{
    public class GameBootstrapper : IStartable
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneService _sceneService;

        public GameBootstrapper(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.Enter<BootstrapState>();
        }
    }
}