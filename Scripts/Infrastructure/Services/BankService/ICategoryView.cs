using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public interface ICategoryView
    {
        RectTransform RectTransform { get; }
        RectTransform Content { get; }
        event Action<ICategoryView> OnDeleted;
        void Initialize(IBankCategory category);
        void SetVisible(bool visible);
        void Delete();
    }
}