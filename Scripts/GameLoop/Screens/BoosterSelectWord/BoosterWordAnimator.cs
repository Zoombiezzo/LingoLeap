using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectWord
{
    public class BoosterWordAnimator : MonoBehaviour
    {
        [SerializeField] private BoosterWordAnimationView _prefab;
        [SerializeField] private BoosterWordPenAnimationView _penAnimationPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _contentPens;
        [SerializeField] private List<BoosterWordAnimationView> _usedViews = new(16);
        [SerializeField] private List<BoosterWordPenAnimationView> _usedPens = new(16);
        [SerializeField] private float _duration = 1f;
        
        private ObjectPool<BoosterWordAnimationView> _pool;
        private ObjectPool<BoosterWordPenAnimationView> _poolPens;
        public float Duration => _duration;

        private void Awake()
        {
            _pool = new ObjectPool<BoosterWordAnimationView>(
                CreateBoosterCharView,
                GetBoosterCharView,
                ReleaseBoosterCharView,
                DestroyBoosterCharView);
            
            _poolPens = new ObjectPool<BoosterWordPenAnimationView>(
                CreatePenAnimationView,
                GetPenCharView,
                ReleasePenCharView,
                DestroyPenCharView);
        }

        private void DestroyBoosterCharView(BoosterWordAnimationView obj) => Destroy(obj.gameObject);
        private void ReleaseBoosterCharView(BoosterWordAnimationView obj) => obj.gameObject.SetActive(false);
        private void GetBoosterCharView(BoosterWordAnimationView obj) => obj.gameObject.SetActive(true);
        private BoosterWordAnimationView CreateBoosterCharView()
        {
            var view = Instantiate(_prefab, _content);
            view.Register(this);
            _usedViews.Add(view);
            return view;
        }
        
        private void DestroyPenCharView(BoosterWordPenAnimationView obj) => Destroy(obj.gameObject);
        private void ReleasePenCharView(BoosterWordPenAnimationView obj) => obj.gameObject.SetActive(false);
        private void GetPenCharView(BoosterWordPenAnimationView obj) => obj.gameObject.SetActive(true);
        private BoosterWordPenAnimationView CreatePenAnimationView()
        {
            var view = Instantiate(_penAnimationPrefab, _contentPens);
            view.Register(this);
            _usedPens.Add(view);
            return view;
        }

        [Button]
        public Sequence PlayAnimation(RectTransform target, out float duration)
        {
            var view = _pool.Get();
            duration = view.Duration;
            return view.PlayAnimation(target);
        }

        public Sequence PlayPenAnimation(List<RectTransform> targets, float durationTranslate, float durationOnTarget)
        {
            var pen = _poolPens.Get();
            return pen.PlayAnimation(targets, durationTranslate, durationOnTarget);
        }

        public void Release(BoosterWordAnimationView boosterCharAnimationView)
        {
            _pool.Release(boosterCharAnimationView);
        }
        
        public void Release(BoosterWordPenAnimationView boosterCharAnimationView)
        {
            _poolPens.Release(boosterCharAnimationView);
        }

        public void ReleaseAll()
        {
            foreach (var view in _usedViews) 
                _pool.Release(view);

            foreach (var pen in _usedPens) 
                _poolPens.Release(pen);
            
            _usedViews.Clear();
            _usedPens.Clear();
        }
    }
}