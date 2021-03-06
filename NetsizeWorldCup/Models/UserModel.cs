﻿using System;
using System.Linq;

namespace NetsizeWorldCup.Models
{
    public class UserModel
    {
        public string Player { get; set; }
        public decimal Score { get; set; }
        public int BetCount { get; set; }

        public int GoodGuess { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

        public string TimeZoneInfoId { get; set; }
        private TimeZoneInfo _timeZone;
        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (_timeZone == null)
                    _timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault<TimeZoneInfo>(i => i.Id == TimeZoneInfoId);

                if (_timeZone == null)
                    return TimeZoneInfo.Local;

                return _timeZone;
            }
            set
            {
                TimeZoneInfoId = value.Id;
                _timeZone = value;
            }
        }
    }
}