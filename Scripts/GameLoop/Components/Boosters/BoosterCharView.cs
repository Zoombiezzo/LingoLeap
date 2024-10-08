using System;
using _Client.Scripts.Tools.Animation;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Client.Scripts.GameLoop.Components.Boosters
{
    public class BoosterCharView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _selectedPanel;
        [SerializeField] private UiAnimation _selectAnimation;
        [SerializeField] private UiAnimation _deselectAnimation;
        [SerializeField] private ParticleImage _selectParticleIdle;
        [SerializeField] private ParticleImage _selectParticle;
        
        private State _state;
        
        public RectTransform RectTransform => _rectTransform;
        public State State => _state;
        public event Action<BoosterCharView> OnClicked;

        public void SetState(State state)
        {
            _state = state;
            
            switch (state)
            {
                case State.Selected:
                    PlayParticleIdle();
                    break;
                case State.None:
                    StopParticleIdle();
                    break;
            }
        }
        
        public Sequence SelectAnimation()
        {
            StopAllAnimations();
            _selectParticle?.Play();
            return _selectAnimation.Play();
        }

        public Sequence DeselectAnimation()
        {
            StopAllAnimations();
            return _deselectAnimation.Play();
        }

        public void Clear()
        {
            _selectedPanel.alpha = 0f;
            _state = State.None;
            StopAllAnimations();
        }

        private void PlayParticleIdle()
        {
            _selectParticleIdle?.Play();
        }
        
        private void StopParticleIdle()
        {
            _selectParticleIdle?.Stop();
            _selectParticleIdle?.Clear();
        }

        private void StopAllAnimations()
        {
            _selectAnimation.Stop();
            _deselectAnimation.Stop();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
    }
}