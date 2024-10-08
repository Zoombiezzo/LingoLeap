using System;
using System.Collections.Generic;
using SQLite;

namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    public class WordsDatabaseService : IWordsDatabaseService
    {
        private readonly string _databasePath;
        private readonly string _language;

        public WordsDatabaseService(string databasePath, string language)
        {
            _databasePath = databasePath;
            _language = language;
        }
        
        public bool Contains(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;
            
            using var db = new SQLiteConnection(_databasePath);
            var result = db.QueryScalars<int>($"SELECT COUNT(*) FROM {_language} WHERE Value = ?", word.ToLower());

            if(result.Count == 0)
                return false;
            
            return result[0] > 0;
        }

        public List<T> QueryScalars<T>(IQueryBuilder queryBuilder)
        {
            var query = queryBuilder.Query;
            
            if (string.IsNullOrEmpty(query))
            {
                query = queryBuilder.Build();
                
                if (string.IsNullOrEmpty(query))
                    return new List<T>();
            }
            
            using var db = new SQLiteConnection(_databasePath);
            var result = db.QueryScalars<T>(query);
            
            return result;
        }
        
        
    }
}