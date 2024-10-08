using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectChar
{
    public class BoosterCharAnimator : MonoBehaviour
    {
        [SerializeField] private BoosterCharAnimationView _prefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _from;
        [SerializeField] private List<BoosterCharAnimationView> _usedViews = new(16);
        
        private ObjectPool<BoosterCharAnimationView> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<BoosterCharAnimationView>(
                CreateBoosterCharView,
                GetBoosterCharView,
                ReleaseBoosterCharView,
                DestroyBoosterCharView);
        }

        private void DestroyBoosterCharView(BoosterCharAnimationView obj) => Destroy(obj.gameObject);
        private void ReleaseBoosterCharView(BoosterCharAnimationView obj) => obj.gameObject.SetActive(false);
        private void GetBoosterCharView(BoosterCharAnimationView obj) => obj.gameObject.SetActive(true);
        private BoosterCharAnimationView CreateBoosterCharView()
        {
            var view = Instantiate(_prefab, _content);
            view.Register(this);
            _usedViews.Add(view);
            return view;
        }

        [Button]
        public Sequence PlayAnimation(RectTransform target, out float duration)
        {
            var view = _pool.Get();
            duration = view.Duration;
            return view.PlayAnimation(_from, target);
        }

        public void Release(BoosterCharAnimationView boosterCharAnimationView)
        {
            _pool.Release(boosterCharAnimationView);
        }

        public void ReleaseAll()
        {
            foreach (var view in _usedViews) 
                _pool.Release(view);
        }
    }
}