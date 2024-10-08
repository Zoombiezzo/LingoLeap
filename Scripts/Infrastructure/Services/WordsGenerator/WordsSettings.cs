#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.WordsGenerator
{
    [Serializable]
    public class WordsSettings
    {
        [FoldoutGroup("@Title")]
        [SerializeField] [LabelText("Длина слова")]
        private int _length;

        [FoldoutGroup("@Title")]
        [SerializeField] [LabelText("Случайное распределение")]
        private bool _isRandom;
            
        [FoldoutGroup("@Title")]
        [Range(0f, 1f)]
        [SerializeField] [LabelText("Процентное соотношение")]
        private float _percentage;
        
        [FoldoutGroup("@Title")]
        [SerializeField]
        [HideIf("@IsEmpty")] [LabelText("Слова")]
        private List<string> _words = new List<string>();
        
        private string Title => $"{_length} ({_percentage * 100}%) Рандом: {(_isRandom ? "Да" : "Нет")} Слова: {_words.Count}";
        private bool IsEmpty => _words.Count == 0;
        
        public int Length => _length;
        public float Percentage => _percentage;
        public IReadOnlyList<string> Words => _words;
        public bool IsRandom => _isRandom;

        public void SetWords(List<string> words)
        {
            _words = words;
        }
        
        public void SetPercentage(float percentage)
        {
            _percentage = percentage;
        }
    }
}
#endif