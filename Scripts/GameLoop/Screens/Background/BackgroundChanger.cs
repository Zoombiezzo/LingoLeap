using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.Background
{
    public class BackgroundChanger : IBackgroundChanger
    {
        private readonly BackgroundPresenter _presenter;
        private readonly MonoBehaviour _runner;
        private List<string> _locationChange = new(2);

        private Coroutine _coroutine;

        public BackgroundChanger(BackgroundPresenter presenter, MonoBehaviour runner)
        {
            _presenter = presenter;
            _runner = runner;
        }

        public void Change(string locationId)
        {
            _locationChange.Add(locationId);
            
            if(_coroutine != null)
                return;
            
            _coroutine = _runner.StartCoroutine(ChangeBackground());
        }

        private IEnumerator ChangeBackground()
        {
            while (_locationChange.Count > 0)
            {
                var backgroundId = _locationChange[^1];
                _locationChange.Clear();
                var task = _presenter.ChangeBackground(backgroundId);
                
                while (!task.IsCompleted)
                    yield return null;
            }
            
            _runner.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}