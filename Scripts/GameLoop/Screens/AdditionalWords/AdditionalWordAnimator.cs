using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordAnimator : MonoBehaviour
    {
        [SerializeField] private AdditionalWordAnimationView _prefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private float _duration;
        [SerializeField] private List<AdditionalWordAnimationView> _usedViews = new(4);
        [SerializeField] private List<AdditionalWordAnimationView> _freeViews = new(4);
        
        public float Duration => _duration;

        public void PlayAnimation(RectTransform from, RectTransform target)
        {
           var view = GetFreeView();
           view.PlayAnimation(from, target);
        }

        public void Clear()
        {
            while (_usedViews.Count > 0)
            {
                var view = _usedViews[^1];
                view.Clear();
                Free(view);
            }

            _usedViews.Clear();
        }

        private AdditionalWordAnimationView GetFreeView()
        {
            AdditionalWordAnimationView view = null;
            
            if (_freeViews.Count == 0)
            {
                view = Instantiate(_prefab, _content);
                view.AttachAnimator(this);
            }
            else
            {
                view = _freeViews[^1];
                _freeViews.RemoveAt(_freeViews.Count - 1);
                _usedViews.Add(view);
            }
            
            return view;
        }

        internal void Free(AdditionalWordAnimationView view)
        {
            _usedViews.Remove(view);
            _freeViews.Add(view);
        }
    }
}