using System;
using System.Collections;
using _Client.Scripts.Infrastructure.Services.TutorialService.Initialize;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _Client.Scripts.Infrastructure.Services.TutorialService.Executors
{
    public class TutorialExecutorUI : MonoBehaviour, ITutorialExecutor
    {
        [SerializeField] private string _id;
        [SerializeField] private Window _window;
        [SerializeField] private RectTransform _root;
        [SerializeField] private MonoInitializeData _initializeData;
        
        private ITutorialService _tutorialService;
        private Coroutine _coroutine;
        
        private ITutorialStep _currentStep;
        private ITutorialConfig _config;
        private ITutorialData _data;
        
        public string Id => _id;
        public bool IsRunning { get; private set; }
        
        [Inject]
        public void Construct(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
            _tutorialService.RegisterTutorialExecutor(this);
        }
        
        public bool Execute(ITutorialConfig config, ITutorialData data)
        {
            if (IsRunning)
                return false;

            if (data.IsCompleted)
                return false;
            
            _config = config;
            _data = data;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            
            _coroutine = StartCoroutine(ExecuteCoroutine());
            return true;
        }

        private IEnumerator ExecuteCoroutine()
        {
            IsRunning = true;

            foreach (var step in _config.Steps)
            {
                if(_data.IsStepCompleted(step.Id))
                    continue;
                
                if(step is MonoTutorialStep monoStep == false)
                    continue;
                
                _currentStep = Instantiate(monoStep, _root);

                yield return _currentStep.Initialize(_initializeData).AsUniTask().ToCoroutine();;
                yield return new WaitUntil(() => _currentStep.IsPossibleStart);
                _window.Show();
                yield return _currentStep.Create().AsUniTask().ToCoroutine();
                yield return _currentStep.Show().AsUniTask().ToCoroutine();
                yield return new WaitUntil(() => _currentStep.IsComplete);
                _data.CompleteStep(_currentStep.Id);
                _window.Hide();
                _currentStep.Delete();
            }
            
            _window.Hide();
            IsRunning = false;
            _data.CompleteTutorial();
            _tutorialService.FreeExecutor(this);
            _tutorialService.ReleaseTutorial(_config.Id);
            _tutorialService.Save();
        }

        public void Reset()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            
            if (_currentStep != null)
            {
                _currentStep.Delete();
                _currentStep = null;
            }
            
            IsRunning = false;
            
            _window.Hide();
            _tutorialService.FreeExecutor(this);
        }

        public void Dispose()
        {
            Reset();
            _tutorialService.UnregisterTutorialExecutor(this);

            if (_config != null)
            {
                _tutorialService.ReleaseTutorial(_config.Id);
            }
        }

        private void OnDestroy() => Dispose();
    }
}