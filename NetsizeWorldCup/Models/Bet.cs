using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using NetsizeWorldCup.Models;

namespace NetsizeWorldCup
{
    public class Bet : Ownership
    {
        public virtual Game Game { get; set; }

        public int Forecast { get; set; }
        public int Amount { get; set; }

        public override string DisplayName
        {
            get { return base.Owner.UserName + " - " + Game.DisplayName; }
        }
    }
}
