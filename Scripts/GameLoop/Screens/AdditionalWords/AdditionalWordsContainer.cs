using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordsContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private AdditionalWordView _prefab;
        
        [SerializeField] private List<AdditionalWordView> _additionalWordsViews = new List<AdditionalWordView>();
        [SerializeField] private List<AdditionalWordView> _freeAdditionalWordsViews = new List<AdditionalWordView>();

        private Dictionary<string, AdditionalWordView> _wordsMap = new Dictionary<string, AdditionalWordView>();
        
        public void AddWord(string word)
        {
            if (_wordsMap.TryGetValue(word, out var additionalWordView) == false)
            {
                additionalWordView = GetAdditionalWordView();
                additionalWordView.Show();
                _wordsMap.Add(word, additionalWordView);
            }
            
            additionalWordView.SetText(word);
        }

        public void AddWords(List<string> words)
        {
            HideAllWords();
            
            foreach (var word in words)
            {
                AddWord(word);
            }
        }

        private void HideAllWords()
        {
            foreach (var additionalWordView in _additionalWordsViews)
            {
                additionalWordView.Hide();
                _freeAdditionalWordsViews.Add(additionalWordView);
            }

            _additionalWordsViews.Clear();
            _wordsMap.Clear();
        }
        
        private AdditionalWordView GetAdditionalWordView()
        {
            AdditionalWordView additionalWordView = null;
            
            if (_freeAdditionalWordsViews.Count > 0)
            {
                additionalWordView = _freeAdditionalWordsViews[^1];
                _freeAdditionalWordsViews.RemoveAt(_freeAdditionalWordsViews.Count - 1);
            }
            else
            {
                additionalWordView = Instantiate(_prefab, _container);
            }

            _additionalWordsViews.Add(additionalWordView);
            return additionalWordView;
        }
    }
}