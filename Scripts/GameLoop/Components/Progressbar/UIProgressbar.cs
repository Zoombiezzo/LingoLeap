using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Progressbar
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIProgressbar : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public virtual void SetProgress(float progress, bool animatie = false)
        {

        }

        public virtual void SetProgress(int count, bool animatie = false)
        {

        }

        public virtual void SetProgress(int count, int maxCount, bool animatie = false)
        {

        }

        public virtual void Show(bool animate = false)
        {
            _canvasGroup.alpha = 1f;
        }
        
        public virtual void Hide(bool animate = false)
        {
            _canvasGroup.alpha = 0f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _canvasGroup ??= GetComponent<CanvasGroup>();
        }
#endif
    }
}