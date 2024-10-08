using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Tools.FlexibleGridLayoutGroup
{
    [AddComponentMenu("Layout/Flexible Grid Layout Group")]
    public class FlexibleGridLayoutGroup : LayoutGroup
    {
        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        [SerializeField] protected Axis m_StartAxis = Axis.Horizontal;

        public Axis startAxis
        {
            get { return m_StartAxis; }
            set { SetProperty(ref m_StartAxis, value); }
        }

        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

        public Vector2 spacing
        {
            get { return m_Spacing; }
            set { SetProperty(ref m_Spacing, value); }
        }

        [System.NonSerialized] private List<(RectTransform, Vector2, Vector2)> _rectChildrenPositions = new(16);
        [System.NonSerialized] private Vector2 _preferredSize = Vector2.zero;

        public override void CalculateLayoutInputVertical()
        {
            _preferredSize = CalculateRequiredSpace();
            SetLayoutInputForAxis(_preferredSize.y, _preferredSize.y, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        private Vector2 CalculateRequiredSpace()
        {
            _rectChildrenPositions ??= new List<(RectTransform, Vector2, Vector2)>(16);
            _rectChildrenPositions.Clear();

            var size = rectTransform.rect.size;
            var width = size.x;
            var height = size.y;

            var maxPreferredSize =
                startAxis == Axis.Horizontal ? width - padding.horizontal : height - padding.vertical;

            var lastPosition = Vector2.zero;
            var preferredSize = Vector2.zero;

            foreach (var rectChild in rectChildren)
            {
                var sizeChild = new Vector2(LayoutUtility.GetPreferredWidth(rectChild),
                    LayoutUtility.GetPreferredHeight(rectChild));
                var endPosition = startAxis == Axis.Horizontal
                    ? new Vector2(lastPosition.x + sizeChild.x, lastPosition.y)
                    : new Vector2(lastPosition.x, lastPosition.y + sizeChild.y);

                var childPosition = lastPosition;

                if (startAxis == Axis.Horizontal)
                {
                    if (endPosition.x > maxPreferredSize)
                        childPosition = new Vector2(0, preferredSize.y);
                }
                else
                {
                    if (endPosition.y > maxPreferredSize)
                        childPosition = new Vector2(preferredSize.x, 0);
                }

                _rectChildrenPositions.Add((rectChild, childPosition, sizeChild));

                var endPositionChild = childPosition + sizeChild;

                if (startAxis == Axis.Horizontal)
                    endPositionChild.y += spacing.y;
                else
                    endPositionChild.x += spacing.x;

                if (preferredSize.x < endPositionChild.x)
                    preferredSize.x = endPositionChild.x;

                if (preferredSize.y < endPositionChild.y)
                    preferredSize.y = endPositionChild.y;

                lastPosition = childPosition;

                if (startAxis == Axis.Horizontal)
                    lastPosition.x += sizeChild.x + spacing.x;
                else
                    lastPosition.y += sizeChild.y + spacing.y;
            }

            if (startAxis == Axis.Horizontal)
                preferredSize.y -= spacing.y;
            else
                preferredSize.x -= spacing.x;

            return preferredSize;
        }

        private void SetCellsAlongAxis(int axis)
        {
            var rectChildrenCount = rectChildren.Count;
            if (axis == 0)
            {
                for (int i = 0; i < rectChildrenCount; i++)
                {
                    RectTransform rect = rectChildren[i];

                    m_Tracker.Add(this, rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = new Vector2(LayoutUtility.GetPreferredWidth(rect),
                        LayoutUtility.GetPreferredHeight(rect));
                }

                return;
            }

            _preferredSize = CalculateRequiredSpace();

            Vector2 lastCellsStartOffset = new Vector2(
                GetStartOffset(0, _preferredSize.x),
                GetStartOffset(1, _preferredSize.y)
            );
            foreach (var (rect, lastPos, size) in _rectChildrenPositions)
            {
                SetChildAlongAxis(rect, 0, lastPos.x + lastCellsStartOffset.x, size.x);
                SetChildAlongAxis(rect, 1, lastPos.y + lastCellsStartOffset.y, size.y);
            }

            //SetLayoutInputForAxis(0, requiredSpace.x, -1, 0);
            //SetLayoutInputForAxis(0, requiredSpace.y, -1, 1);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying == false)
                SetCellsAlongAxis((int)m_StartAxis);
        }
#endif
    }
}