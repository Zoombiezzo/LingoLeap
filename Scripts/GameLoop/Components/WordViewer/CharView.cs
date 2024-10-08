using _Client.Scripts.Tools.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.WordViewer
{
    public class CharView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private LayoutElement _layoutElement;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private UiAnimation _showAnimation;        
        
        private char _char;
        
        public char Char => _char;
        
        public void SetChar(char c)
        {
            _char = c;
            _text.text = c.ToString();
        }

        public void Show(bool animate = false)
        {
            StopAnimation();
            _layoutElement.ignoreLayout = false;

            if (animate)
            {
                _showAnimation?.Play();
            }
            else
            {
                _canvasGroup.alpha = 1f;
            }
        }

        public void Hide()
        {
            StopAnimation();
            _canvasGroup.alpha = 0f;
            _layoutElement.ignoreLayout = true;
        }

        public void Clear()
        {
            _text.text = string.Empty;
        }
        
        private void StopAnimation()
        {
            _showAnimation?.Stop();
        }
    }
}