using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation : UiAnimation
    {
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/ALPHA ANIMATION")]
        [OnValueChanged("ChangeIsAlphaAnimationChanged")]
        private bool _isAlphaAnimation;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/ALPHA ANIMATION")]
        [ShowIf("_isAlphaAnimation")]
        private AlphaUIAnimationCustom _alphaUIAnimation;

        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/COLOR IMAGE ANIMATION")]
        [OnValueChanged("ChangeIsColorImageAnimationChanged")]
        private bool _isColorImageAnimation;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/COLOR IMAGE ANIMATION")] 
        [ShowIf("_isColorImageAnimation")] 
        private ImageColorUIAnimationCustom _imageColorUIAnimation;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/SCALE ANIMATION")]
        [OnValueChanged("ChangeIsScaleAnimationChanged")]
        private bool _isScaleAnimation;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/SCALE ANIMATION")] 
        [ShowIf("_isScaleAnimation")]
        private ScaleUIAnimationCustom _scaleUIAnimation;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/MOVE ANIMATION")]
        [OnValueChanged("ChangeIsMoveAnimationChanged")]
        private bool _isMoveAnimation;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/MOVE ANIMATION")] 
        [ShowIf("_isMoveAnimation")]
        private MoveUIAnimationCustom _moveUIAnimation;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/RELATIVE MOVE ANIMATION")]
        [OnValueChanged("ChangeIsRelativeMoveAnimationChanged")]
        private bool _isRelativeMoveAnimation;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/RELATIVE MOVE ANIMATION")] 
        [ShowIf("_isRelativeMoveAnimation")]
        private RelativeMoveUIAnimationCustom _relativeMoveUIAnimation;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/ATTRACTION MOVE ANIMATION")]
        [OnValueChanged("ChangeIsAttractionMoveAnimationChanged")]
        private bool _isMoveAttraction;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/ATTRACTION MOVE ANIMATION")] 
        [ShowIf("_isMoveAttraction")]
        private AttractionMoveUIAnimationCustom _attractionMoveUIAnimation;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/RELATIVE ROTATION ANIMATION")]
        [OnValueChanged("ChangeIsRelativeRotationAnimationChanged")]
        private bool _isRelativeRotationAnimation;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/RELATIVE ROTATION ANIMATION")] 
        [ShowIf("_isRelativeRotationAnimation")]
        private RelativeRotationUIAnimationCustom _relativeRotationUIAnimation;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/LOOP")]
        [OnValueChanged("ChangeIsLoopChanged")]
        private bool _isLoop;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/LOOP")] 
        [ShowIf("_isLoop")]
        private DelayUIAnimationCustom _loopDelay;
        
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/START DELAY")]
        [OnValueChanged("ChangeIsStartDelayChanged")]
        private bool _isStartDelay;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/START DELAY")] 
        [ShowIf("_isStartDelay")]
        private DelayUIAnimationCustom _startedDelay;
        [SerializeField] [FoldoutGroup("SETTINGS")] [FoldoutGroup("SETTINGS/PLAY ON START")]
        private bool _playOnStart;

        private Action _onComplete;
        private Sequence _sequence;

        private void Start()
        {
            if (_playOnStart) 
                Play();
        }

        public override Sequence Play(Action onComplete = null)
        {
            Stop();
            
            _onComplete = onComplete;
            
            IsPlaying = true;
            IsFinished = false;

            var mainSequence = DOTween.Sequence();
            
            if (_isAlphaAnimation)
            {
                _alphaUIAnimation.Initialize();
                mainSequence.Join(_alphaUIAnimation.Create());
            }
            
            if (_isColorImageAnimation)
            {
                _imageColorUIAnimation.Initialize();
                mainSequence.Join(_imageColorUIAnimation.Create());
            }
            
            if (_isScaleAnimation)
            {
                _scaleUIAnimation.Initialize();
                mainSequence.Join(_scaleUIAnimation.Create());
            }

            if (_isMoveAnimation)
            {
                _moveUIAnimation.Initialize();
                mainSequence.Join(_moveUIAnimation.Create());
            }
            
            if (_isRelativeMoveAnimation)
            {
                _relativeMoveUIAnimation.Initialize();
                mainSequence.Join(_relativeMoveUIAnimation.Create());
            }
            
            if (_isMoveAttraction)
            {
                _attractionMoveUIAnimation.Initialize();
                mainSequence.Join(_attractionMoveUIAnimation.Create());
            }
            
            if (_isRelativeRotationAnimation)
            {
                _relativeRotationUIAnimation.Initialize();
                mainSequence.Join(_relativeRotationUIAnimation.Create());
            }

            var sequence = DOTween.Sequence();

            if (_isStartDelay)
            {
                _startedDelay.Initialize();
                sequence.Append(_startedDelay.Create());
            }

            sequence.Append(mainSequence);

            sequence.SetEase(Ease.Linear);
            
            if (_isLoop)
            {
                _loopDelay.Initialize();
                sequence.Append(_loopDelay.Create());
                sequence.SetLoops(int.MaxValue);
            }
            
            sequence.OnComplete(() =>
            {
                IsPlaying = false;
                IsFinished = true;
                _onComplete?.Invoke();
            });
            

            _sequence = DOTween.Sequence().Append(sequence);
            return _sequence;
        }

        public override void Stop()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Complete();
                _sequence.Kill();
                _onComplete?.Invoke();
            }

            if (IsPlaying || IsFinished)
            {
                ResetValues();
            }

            IsPlaying = false;
            IsFinished = false;
        }

        public void SetTargetAttraction(RectTransform target)
        {
            _attractionMoveUIAnimation?.SetTarget(target);
        }

        private void ResetValues()
        {
            if (_isAlphaAnimation)
                _alphaUIAnimation.Reset();
            
            if (_isColorImageAnimation)
                _imageColorUIAnimation.Reset();
            
            if (_isScaleAnimation)
                _scaleUIAnimation.Reset();
            
            if (_isMoveAnimation)
                _moveUIAnimation.Reset();
            
            if (_isRelativeMoveAnimation)
                _relativeMoveUIAnimation.Reset();
            
            if (_isMoveAttraction)
                _attractionMoveUIAnimation.Reset();
            
            if (_isRelativeRotationAnimation)
                _relativeRotationUIAnimation.Reset();
            
            if (_isLoop)
                _loopDelay.Reset();
            
            if (_isStartDelay)
                _startedDelay.Reset();
        }

        private void OnDestroy()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }
            
            IsPlaying = false;
            IsFinished = false;
        }

#if UNITY_EDITOR
        private void ChangeIsAlphaAnimationChanged() => _alphaUIAnimation = _isAlphaAnimation && _alphaUIAnimation != null ? new AlphaUIAnimationCustom(GetComponent<CanvasGroup>()) : null;

        private void ChangeIsScaleAnimationChanged() => _scaleUIAnimation = _isScaleAnimation && _scaleUIAnimation != null ? new ScaleUIAnimationCustom(GetComponent<RectTransform>()) : null;

        private void ChangeIsColorImageAnimationChanged() => _imageColorUIAnimation = _isColorImageAnimation && _imageColorUIAnimation != null ? new ImageColorUIAnimationCustom(GetComponent<Image>()) : null;

        private void ChangeIsMoveAnimationChanged() => _moveUIAnimation = _isMoveAnimation && _moveUIAnimation != null ? new MoveUIAnimationCustom(GetComponent<RectTransform>()) : null;
        
        private void ChangeIsRelativeMoveAnimationChanged() => _relativeMoveUIAnimation = _isRelativeMoveAnimation && _relativeMoveUIAnimation != null ? new RelativeMoveUIAnimationCustom(GetComponent<RectTransform>()) : null;
        
        private void ChangeIsAttractionMoveAnimationChanged() => _attractionMoveUIAnimation = _isMoveAttraction && _attractionMoveUIAnimation != null ? new AttractionMoveUIAnimationCustom(GetComponent<RectTransform>()) : null;
        
        private void ChangeIsRelativeRotationAnimationChanged() => _relativeRotationUIAnimation = _isRelativeRotationAnimation && _relativeRotationUIAnimation != null ? new RelativeRotationUIAnimationCustom(GetComponent<RectTransform>()) : null;
        
        private void ChangeIsLoopChanged() => _loopDelay = _isLoop && _loopDelay != null ? new DelayUIAnimationCustom() : null;
        
        private void ChangeIsStartDelayChanged() => _startedDelay = _isStartDelay && _startedDelay != null ? new DelayUIAnimationCustom() : null;
#endif
    }
}