using System;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Tools;
using R3;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Boosters
{
    public class BoosterButtonView : MonoBehaviour
    {
        [SerializeField] private CounterField _counterField;
        [SerializeField] private AnimationButton _animationButton;
        [SerializeField] private GameObject _plusIcon;
        
        private IDisposable _clickDisposable;
        public event Action OnClick;
        
        public void SetValue(int count, bool animate = false)
        {
            _counterField.SetValue(count, animate);
            _plusIcon.SetActive(count <= 0);
        }

        public void OnEnable()
        {
            _clickDisposable = _animationButton.OnClick.AsObservable().Subscribe(OnButtonClicked);
        }
        
        public void OnDisable()
        {
            _clickDisposable?.Dispose();
        }
        

        private void OnButtonClicked(Unit _)
        {
            OnClick?.Invoke();
        }
    }
}