using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetsizeWorldCup.Models
{
    public class GameResults
    {
        public Dictionary<string, UserResult> Results { get; set; }
        public int GamesPlayedCount { get; set; }
    }

    public class UserResult
    {
        public decimal Score;
        public int Count;
    }
}