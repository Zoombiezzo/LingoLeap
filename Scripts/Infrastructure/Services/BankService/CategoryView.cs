using System;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public abstract class CategoryView : MonoBehaviour, ICategoryView
    {
        [SerializeField] protected RectTransform _rectTransform;
        [SerializeField] protected RectTransform _content;
        [SerializeField] protected CanvasGroup _canvasGroupLayout;
        public RectTransform RectTransform => _rectTransform;
        public RectTransform Content => _content;
        public event Action<ICategoryView> OnDeleted;

        protected IBankCategory _bankCategory;
        
        public virtual void Initialize(IBankCategory category)
        {
            _bankCategory = category;
        }
        
        public virtual void SetVisible(bool visible)
        {
            _canvasGroupLayout.alpha = visible ? 1 : 0;
            _canvasGroupLayout.blocksRaycasts = visible;
            _canvasGroupLayout.interactable = visible;
        }
        
        public virtual void Delete()
        {
            OnDeleted?.Invoke(this);
        }
    }
}