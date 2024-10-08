using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools
{
    [Serializable]
    public class DateTimeRange
    {
        [SerializeField] [FoldoutGroup("@Title")] [FoldoutGroup("@Title/@TitleFrom", false)] [HideLabel]
        private DateTimeInfo _dateFrom;
        [SerializeField] [FoldoutGroup("@Title")] [FoldoutGroup("@Title/@TitleTo", false)] [HideLabel]
        private DateTimeInfo _dateTo;

        public DateTimeInfo From => _dateFrom;
        public DateTimeInfo To => _dateTo;

        public double TotalSeconds()
        {
            var totalSeconds = (_dateTo.GetTime() - _dateFrom.GetTime()).TotalSeconds;
            return totalSeconds;
        }

        public bool IsIncluded(DateTime time)
        {
            if (time < _dateFrom) return false;
            if (_dateTo < time) return false;
            if (_dateTo == time) return false;

            return true;
        }

#if UNITY_EDITOR
        private string Title => $"{_dateFrom} - {_dateTo} | Sec:({TotalSeconds()})";
        private string TitleFrom => $"От: {_dateFrom}";
        private string TitleTo => $"До: {_dateTo}";
#endif
    }
}