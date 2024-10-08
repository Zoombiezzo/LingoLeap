using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Tools.Layout
{
    [ExecuteAlways]
    public class VerticalOrHorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        [SerializeField] protected LayoutDirection m_layoutDirection = LayoutDirection.Vertical;

        public LayoutDirection direction
        {
            get => m_layoutDirection;
            set => SetProperty(ref m_layoutDirection, value);
        }

        protected VerticalOrHorizontalLayoutGroup()
        {
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, m_layoutDirection == LayoutDirection.Vertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, m_layoutDirection == LayoutDirection.Vertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, m_layoutDirection == LayoutDirection.Vertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, m_layoutDirection == LayoutDirection.Vertical);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
        }

        protected override void Update()
        {
           base.Update();
        }
#endif
    }
}