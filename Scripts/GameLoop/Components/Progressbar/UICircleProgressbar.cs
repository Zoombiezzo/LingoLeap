using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.Progressbar
{
    public class UICircleProgressbar : UIProgressbar
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private bool _fillClockwise;
        [SerializeField] private Image.Origin360 _fillOrigin;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float _progress;
        
        private Sequence _progressSequence;

        public override void SetProgress(float progress, bool animatie = false)
        {
            if (_progressSequence != null && _progressSequence.IsPlaying())
            {
                _progressSequence.Kill();
            }

            if (animatie)
            {
                _progressSequence = DOTween.Sequence();

                if (_progress > progress)
                {
                    _progressSequence.Append(DOVirtual.Float(_progress, 1f, _duration,
                            value => _fillImage.fillAmount = value))
                        .Append(DOVirtual.Float(1f, 0f, 0f,
                            value => _fillImage.fillAmount = value))
                        .Append(DOVirtual.Float(0f, progress, _duration,
                            value =>
                            {
                                _progress = value;
                                _fillImage.fillAmount = value;
                            }));
                }
                else
                {
                    _progressSequence
                        .Append(DOVirtual.Float(_progress, progress, _duration,
                            value =>
                            {
                                _progress = value;
                                _fillImage.fillAmount = value;
                            }));
                }

                _progressSequence.OnComplete(() =>
                {
                    _progress = progress;
                    _fillImage.fillAmount = _progress;
                });
            }
            else
            {
                _fillImage.fillAmount = progress;

                _progress = progress;
                _fillImage.fillAmount = _progress;
            }
        }
        
        public override void SetProgress(int count, int maxCount, bool animatie = false)
        {
            SetProgress((float) count / maxCount, animatie);
        }

#if UNITY_EDITOR
        [SerializeField] [Range(0f, 1f)]private float _nextProgress = 0f;
        
        [Button]
        private void UpdateProgress()
        {
            SetProgress(_nextProgress, true);
        }
        

        private void OnValidate()
        {
            if (_fillImage != null)
            {
                if (_fillImage.type != Image.Type.Filled)
                {
                    _fillImage.type = Image.Type.Filled;
                    _fillImage.fillMethod = Image.FillMethod.Radial360;
                }

                _fillImage.fillClockwise = _fillClockwise;
                _fillImage.fillOrigin = (int)_fillOrigin;
                if (Application.isPlaying == false)
                {
                    _fillImage.fillAmount = _progress;
                }
            }
        }
#endif
    }
}