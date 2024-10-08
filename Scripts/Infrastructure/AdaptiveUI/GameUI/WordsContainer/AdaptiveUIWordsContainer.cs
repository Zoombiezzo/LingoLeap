using UnityEngine;

namespace _Client.Scripts.Infrastructure.AdaptiveUI.GameUI.WordsContainer
{
    [RequireComponent(typeof(GameLoop.Components.WordsContainer.WordsContainer))]
    public class AdaptiveUIWordsContainer : AdaptiveUIElement
    {
        [SerializeField] private GameLoop.Components.WordsContainer.WordsContainer _wordsContainer;
        
        public override void ChangeOrientation(AdaptiveUIOrientation orientation, bool force = false)
        {
            if(_isEnabled == false)
                return;
            
            if (_currentOrientation == orientation && force == false)
                return;
            
            _wordsContainer.RecalculateSizeCoroutine();
            
            base.ChangeOrientation(orientation, force);
        }

        public override void ChangeSize(Vector2 size, bool force = false)
        {
            if(_isEnabled == false)
                return;
            
            if(_currentSize == size && force == false)
                return;
            
            _wordsContainer.RecalculateSizeCoroutine();
            
            base.ChangeSize(size, force);
        }
        
#if UNITY_EDITOR
        protected override void Reset()
        {
            if (_wordsContainer == null)
                TryGetComponent(out _wordsContainer);
            
            base.Reset();
        }
#endif
    }
}