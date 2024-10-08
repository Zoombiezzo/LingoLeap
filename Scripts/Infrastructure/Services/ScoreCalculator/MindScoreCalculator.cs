using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;

namespace _Client.Scripts.Infrastructure.Services.ScoreCalculator
{
    public class MindScoreCalculator : IMindScoreCalculator
    {
        private readonly IAdditionalWordsData _additionalWordsData;
        private readonly ILevelProgressData _levelProgressData;

        public MindScoreCalculator(IAdditionalWordsData additionalWordsData, ILevelProgressData levelProgressData)
        {
            _additionalWordsData = additionalWordsData;
            _levelProgressData = levelProgressData;
        }
        
        public int Calculate()
        {
            var additionalLevelRecord = _additionalWordsData.GetLevelRecord();
            var levelRecord = _levelProgressData.GetLevelRecord();
            
            return additionalLevelRecord.OpenedWords.Count + levelRecord.OpenedWords.Count;
        }
    }
}