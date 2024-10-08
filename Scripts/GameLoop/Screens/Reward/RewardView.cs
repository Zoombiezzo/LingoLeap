using _Client.Scripts.Tools.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Reward
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _iconBallon;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Sprite[] _ballonSprites;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private UiAnimation _hideAnimation;
        [SerializeField] private float _duration;
        
        public float Duration => _duration;
        
        public void Initialize(Sprite icon, int count)
        {
            _icon.sprite = icon;
            _countText.text = count.ToString();
            _iconBallon.sprite = _ballonSprites[Random.Range(0, _ballonSprites.Length)];
        }
        
        public void Show(bool animate = false)
        {
            _canvasGroup.alpha = 1;

            if (animate)
            {
                _hideAnimation.Stop();
                _showAnimation.Play();
            }
        }
        
        public void Hide(bool animate = false)
        {
            _canvasGroup.alpha = 0;
            
            if (animate)
            {
                _showAnimation.Stop();
                _hideAnimation.Play();
            }
        }
    }
}