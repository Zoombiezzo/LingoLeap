using System.Collections.Generic;

namespace _Client.Scripts.GameLoop.Data.AdditionalWordsProgress
{
    public interface ILanguageAdditionalWordsRecord
    {
        string Language { get; }
        List<string> OpenedWords { get; }
    }
}