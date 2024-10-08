#if UNITY_EDITOR
using System.Collections.Generic;
using GameSDK.Localization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Client.Scripts.Infrastructure.Services.WordsGenerator
{
    [CreateAssetMenu(fileName = "WordsGenerator", menuName = "WordsGenerator", order = 0)]
    public class WordsGeneratorEditor : ScriptableObject
    {
        [FoldoutGroup("Настройки")] [SerializeField]
        private Object _database;
        
        [FoldoutGroup("Настройки")] [SerializeField] [ValueDropdown("GetLanguages")]
        private string _language;
        
        [SerializeField] [HideLabel]
        private GeneralWordGeneratorEditor _generalWordGenerator;
        
        [SerializeField] [HideLabel]
        private SecondaryWordsGeneratorEditor _secondaryWordsGenerator;
        
        public string Language => _language;
        public string DBPath => AssetDatabase.GetAssetPath(_database);

        public GeneralWordGeneratorEditor GeneralWordGenerator => _generalWordGenerator; 
        
        private List<string> GetLanguages()
        {
            var languageProperties = new List<string>(){""};
            foreach (var language in LanguageProperties.Languages)
            {
                languageProperties.Add(language.Code);
            }

            return languageProperties;
        }

        private void OnValidate()
        {
            _generalWordGenerator ??= new GeneralWordGeneratorEditor(this);
            _generalWordGenerator.SetWordsGeneratorEditor(this);
            
            _secondaryWordsGenerator ??= new SecondaryWordsGeneratorEditor(this);
            _secondaryWordsGenerator.SetWordsGeneratorEditor(this);
        }
    }
}
#endif