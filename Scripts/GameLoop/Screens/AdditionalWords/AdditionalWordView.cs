using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private CanvasGroup _canvasGroup;

        public void SetText(string text)
        {
            _text.text = text;
        }
        
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _layoutElement.ignoreLayout = false;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _layoutElement.ignoreLayout = true;
        }
    }
}