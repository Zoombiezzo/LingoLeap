using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Helpers
{
    public static class ComponentsExtensions
    {
        public static Vector2 GetAnchoredPosition(this ScrollRect scrollRect, RectTransform target, Vector2 offsetNormalized)
        {
            var container = scrollRect.content;
            var scrollTransform = scrollRect.transform;
            var containerPosition = (Vector2)scrollTransform.InverseTransformPoint(container.position);
            var locationPosition = (Vector2)scrollTransform.InverseTransformPoint(target.position);

            var delta = containerPosition - locationPosition;
            
            var offsetSize = target.sizeDelta * target.pivot;

            var viewport = scrollRect.viewport;
            var contentSize = container.rect.size;
            var viewportSize = viewport.rect.size;
            var contentPivot = container.pivot;
            var min = -(contentSize * (Vector2.one - container.pivot)) +
                      (Vector2.one - container.anchorMin) * viewportSize;
            var max = (contentSize * contentPivot) - container.anchorMin * viewportSize;

            if (min.x > max.x) 
                (min.x, max.x) = (max.x, min.x);

            if (min.y > max.y) 
                (min.y, max.y) = (max.y, min.y);

            if (scrollRect.horizontal)
            {
                //delta.x += offsetSize.x;
                delta.x += viewportSize.x * offsetNormalized.x;
                delta.x = Mathf.Clamp(delta.x, min.x, max.x);
            }
            else
            {
                delta.x = 0f;
            }
            
            if (scrollRect.vertical)
            {
                //delta.y -= offsetSize.y;
                delta.y -= viewportSize.y * offsetNormalized.y;
                delta.y = Mathf.Clamp(delta.y, min.y, max.y);
            
            }
            else
            {
                delta.y = 0f;
            }

            return delta;
        }
        
        public static void ScrollToElement(this ScrollRect scrollRect, RectTransform target, Vector2 offsetNormalized)
        {
            var container = scrollRect.content;
            scrollRect.velocity = Vector2.zero;
            container.anchoredPosition = GetAnchoredPosition(scrollRect, target, offsetNormalized);
        }

    }
}