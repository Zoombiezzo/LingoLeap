using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Screens.WordsLevel;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.Services.WordsLevelsService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class GameState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;
        private readonly IWordsLevelsService _wordsLevelsService;
        private readonly ILevelProgressData _levelProgressData;
        
        private WordsLevel _wordsLevel;
        private ILevelRecord _levelRecord;

        public GameState(IGameStateMachine stateMachine, ISceneService sceneService, IWordsLevelsService wordsLevelsService, ILevelProgressData levelProgressData)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
            _wordsLevelsService = wordsLevelsService;
            _levelProgressData = levelProgressData;
        }

        public async void Enter()
        {
           WindowsService.TryGetWindow<WordsLevelWindow>(out var window);
           window.Show();
        }

        public void Exit()
        {
            
        }
    }
}