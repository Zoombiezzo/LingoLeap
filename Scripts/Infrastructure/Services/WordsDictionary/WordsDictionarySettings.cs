using System;
using System.Collections;
using Sirenix.OdinInspector;
using SQLite;
using UnityEngine;
using UnityEngine.Networking;

namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    [CreateAssetMenu(menuName = "Words Dictionary/Settings", fileName = "WordsDictionarySettings", order = 0)]
    [Serializable]
    public class WordsDictionarySettings : ScriptableObject
    {
        [SerializeField]
        WordsAssetDatabase _wordsAssetDatabase;

        public WordsAssetDatabase WordsAssetDatabase => _wordsAssetDatabase;

#if UNITY_EDITOR

        [SerializeField] private string _sheedId;
        
        [SerializeField]
        private UnityEngine.Object _outputFolder;
        
        [SerializeField]
        private ImportWordsDictionarySheet[] _sheets;

        private Unity.EditorCoroutines.Editor.EditorCoroutine _importRoutine;

        public string outputFolder => GetOutputFolder();

        private string GetOutputFolder()
        {
            if ( _outputFolder == null )
                return UnityEngine.Application.dataPath;

            return UnityEditor.AssetDatabase.GetAssetPath( _outputFolder );
        }

        [Button]
        public void ImportWords()
        {
            StopImport();
            _importRoutine = Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(ImportWordsRoutine());
        }

        [Button]
        public void StopImport()
        {
            if (_importRoutine != null)
            {
                Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StopCoroutine(_importRoutine);
                _importRoutine = null;
            }
        }
        
        private IEnumerator ImportWordsRoutine()
        {
            var path = GetDatabasePathForQueries(this);

            string newLine = System.Environment.NewLine;

            using var db = new SQLiteConnection(path);
            
            foreach (var sheet in _sheets)
            {
                string url = $"https://docs.google.com/spreadsheets/d/{_sheedId}/export?format=tsv&gid={sheet.SheetId}";

                using UnityWebRequest webRequest = UnityWebRequest.Get( url );

                var asyncOp = webRequest.SendWebRequest();
                
                string title = "Downloading";
                string info = $"Downloading \"{sheet.Localization}\" sheet";
                
                int progressId = UnityEditor.Progress.Start( title, info );
                
                while ( asyncOp.isDone == false )
                {
                    UnityEditor.Progress.Report( progressId, asyncOp.progress );
                    yield return null;
                }
                
                UnityEditor.Progress.Remove( progressId );

                if ( webRequest.result != UnityWebRequest.Result.Success )
                {
                    continue;
                }

                string rawdata = webRequest.downloadHandler.text;

                string[] lines = rawdata.Split(newLine);

                db.Execute($"create table if not exists {sheet.Localization}(Value varchar(32) not null)");
                db.Execute($"delete from {sheet.Localization}");
                
                title = "Importing";
                info = $"Importing \"{sheet.Localization}\" sheet";
                progressId = UnityEditor.Progress.Start( title, info );

                for (var index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    
                    var progress = (float) index / lines.Length;
                    
                    if (string.IsNullOrEmpty(line))
                        continue;

                    db.Execute($"insert or replace into {sheet.Localization}(Value) values (?)",
                        line.Replace("\r", "").Replace(" ", "").ToLower());
                    
                    UnityEditor.Progress.Report( progressId, progress );

                    if (index % 2500 == 0)
                        yield return null;
                }
                
                UnityEditor.Progress.Remove( progressId );
            }

            _importRoutine = null;
        }
        
        
        static string GetDatabasePathForQueries( WordsDictionarySettings settings )
        {
            return $"{settings.outputFolder}/words-database.bytes";
        }
#endif

    }
}