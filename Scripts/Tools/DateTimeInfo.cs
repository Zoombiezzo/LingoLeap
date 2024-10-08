using System;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools
{
    [Serializable]
    public class DateTimeInfo : IEquatable<DateTimeInfo>
    {
        private const int FieldWidth = 20;

        [SerializeField] [HorizontalGroup("DateTime")] [LabelWidth(FieldWidth)]
        [LabelText("Y:")] [OnValueChanged("OnValidate")] [Min(0)]
        private int _year = 1;
        [SerializeField] [HorizontalGroup("DateTime")] [LabelWidth(FieldWidth)]
        [LabelText("M:")] [OnValueChanged("OnValidate")] [Min(1)] [MaxValue(12)]
        private int _month = 1;
        [SerializeField] [HorizontalGroup("DateTime")] [LabelWidth(FieldWidth)]
        [LabelText("D:")] [OnValueChanged("OnValidate")] [Min(1)] [MaxValue(31)]
        private int _day = 1;
        [SerializeField] [HorizontalGroup("DateTime")] [LabelWidth(FieldWidth)]
        [LabelText("h:")] [OnValueChanged("OnValidate")] [Min(0)] [MaxValue(23)]
        private int _hour = 0;
        [SerializeField] [HorizontalGroup("DateTime")] [LabelWidth(FieldWidth)]
        [LabelText("m:")] [OnValueChanged("OnValidate")] [Min(0)] [MaxValue(59)]
        private int _minute = 0;
        [SerializeField] [HorizontalGroup("DateTime")] [LabelWidth(FieldWidth)]
        [LabelText("s:")] [OnValueChanged("OnValidate")] [Min(0)] [MaxValue(59)]
        private int _second = 0;
        
        public static bool operator >(DateTimeInfo left, DateTime right)
        {
            if (left == null) return false;

            if (left._year > right.Year)
                return true;
            if (left._year < right.Year)
                return false;
        
            if (left._month > right.Month)
                return true;
            if (left._month < right.Month)
                return false;

            if (left._day > right.Day)
                return true;
            if (left._day < right.Day)
                return false;

            if (left._hour > right.Hour)
                return true;
            if (left._hour < right.Hour)
                return false;

            if (left._minute > right.Minute)
                return true;
            if (left._minute < right.Minute)
                return false;

            return left._second > right.Second;
        }

        public static bool operator <(DateTimeInfo left, DateTime right)
        {
            if (left == null) return false;

            if (left._year < right.Year)
                return true;
            if (left._year > right.Year)
                return false;
        
            if (left._month < right.Month)
                return true;
            if (left._month > right.Month)
                return false;

            if (left._day < right.Day)
                return true;
            if (left._day > right.Day)
                return false;

            if (left._hour < right.Hour)
                return true;
            if (left._hour > right.Hour)
                return false;

            if (left._minute < right.Minute)
                return true;
            if (left._minute > right.Minute)
                return false;

            return left._second < right.Second;
        }
        
        public static bool operator >(DateTime dateTime, DateTimeInfo info)
        {
            return info < dateTime;
        }

        public static bool operator <(DateTime dateTime, DateTimeInfo info)
        {
            return info > dateTime;
        }

        public static bool operator ==(DateTimeInfo info, DateTime dateTime)
        {
            return info != null &&
                info._year == dateTime.Year &&
                info._month == dateTime.Month &&
                info._day == dateTime.Day &&
                info._hour == dateTime.Hour &&
                info._minute == dateTime.Minute &&
                info._second == dateTime.Second;
        }

        public static bool operator !=(DateTimeInfo info, DateTime dateTime)
        {
            return !(info == dateTime);
        }

        public static bool operator ==(DateTime dateTime, DateTimeInfo info)
        {
            return info == dateTime;
        }

        public static bool operator !=(DateTime dateTime, DateTimeInfo info)
        {
            return !(info == dateTime);
        }

        public DateTime GetTime()
        {
#if UNITY_EDITOR
            OnValidate();
#endif
            return new DateTime(_year, _month, _day, _hour, _minute, _second);
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            OnValidate();
#endif 
            return $"{_day} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_month)} {_year} | {_hour:00}:{_minute:00}:{_second:00}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DateTimeInfo);
        }

        public bool Equals(DateTimeInfo other)
        {
            return other != null &&
                _year == other._year &&
                _month == other._month &&
                _day == other._day &&
                _hour == other._hour &&
                _minute == other._minute &&
                _second == other._second;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_year, _month, _day, _hour, _minute, _second);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _hour = Mathf.Clamp(_hour, 0, 23);
            _minute = Mathf.Clamp(_minute, 0, 59);
            _second = Mathf.Clamp(_second, 0, 59);
            _year = Mathf.Clamp(_year, 0, int.MaxValue);
            _month = Mathf.Clamp(_month, 1, 12);
            _day = Mathf.Clamp(_day, 1, DateTime.DaysInMonth(_year, _month));
        }
#endif        
    }
}