using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    public class WordQueryBuilder : IQueryBuilder
    {
        private int _minLength = 0;
        private int _maxLength = int.MaxValue;
        private string _language = "ru_RU";
        private string _containsChars = "";
        private string _query = "";
        private bool _onlyContainsChars = false;
        private bool _orStatementChars = false;
        private bool _onlyStatementCharsCount = false;

        public string Query => _query;

        public WordQueryBuilder Clear()
        {
            _maxLength = int.MaxValue;
            _minLength = 0;
            _language = "ru_RU";
            _containsChars = "";
            _onlyContainsChars = false;
            _orStatementChars = false;
            _onlyStatementCharsCount = false;
            _query = "";
            return this;
        }
        
        public WordQueryBuilder WithContainsChars(string containsChars)
        {
            _containsChars = containsChars;
            return this;
        }
        
        public WordQueryBuilder WithLanguage(string language)
        {
            _language = language;
            return this;
        }
        
        public WordQueryBuilder WithMinLength(int minLength)
        {
            _minLength = minLength;
            return this;
        }
        
        public WordQueryBuilder WithMaxLength(int maxLength)
        {
            _maxLength = maxLength;
            return this;
        }
        
        public WordQueryBuilder OnlyContainsChars()
        {
            _onlyContainsChars = true;
            return this;
        }
        
        public WordQueryBuilder UseOrStatementChars()
        {
            _orStatementChars = true;
            return this;
        }
        
        public WordQueryBuilder OnlyContainsCharsCount()
        {
            _onlyStatementCharsCount = true;
            return this;
        }
        
        public string Build()
        {
            _query = $"SELECT * FROM {_language} WHERE length(Value) >= {_minLength} AND length(Value) <= {_maxLength}";
            
            if (string.IsNullOrEmpty(_containsChars) == false)
            {
                _query += " AND (";
                var chars = _containsChars.ToLower();
                for (var index = 0; index < chars.Length; index++)
                {
                    var c = chars[index];
                    _query += $"Value LIKE '%{c}%'";
                    
                    if (index < chars.Length - 1)
                    {
                        _query += _orStatementChars ? " OR " : " AND ";
                    }
                }

                _query += ")";

                if (_onlyContainsChars)
                {
                    _query +=  $" AND length({AddReplaceChars("Value", chars, 0)}) = {0}";
                }

                if (_onlyStatementCharsCount)
                {
                    var dictionaryChars = new Dictionary<char, int>(chars.Length);
                    
                    foreach (var c in chars)
                    {
                        if (dictionaryChars.TryAdd(c, 1) == false)
                        {
                            dictionaryChars[c]++;
                        }
                    }
                    
                    foreach (var c in chars)
                    {
                        if(dictionaryChars.TryGetValue(c, out var count) == false)
                            continue;
                        
                        _query += $" AND length(Value) - length(replace(Value, '{c}', '')) <= {count}";
                    }
                }
            }
            
            return _query;
        }

        private string AddReplaceChars(string column, string chars, int numberOfChars)
        {
            if (chars.Length - 1 == numberOfChars) return $"replace({column}, '{chars[numberOfChars]}', '')";
            
            return $"replace({AddReplaceChars(column, chars, numberOfChars + 1)}, '{chars[numberOfChars]}', '')";
        }
    }
}