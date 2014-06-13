using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetsizeWorldCup
{
    public class Game : Entity
    {
        public virtual Team Local { get; set; }
        public virtual Team Visitor { get; set; }

        public virtual Phase Phase { get; set; }

        public DateTime StartDate { get; set; }

        [NotMapped]
        public DateTime EndDate { get { return StartDate.AddMinutes(105); } }

        public string Location { get; set; }
        public string TVBroadcast { get; set; }

        public int? Result { get; set; }

        public decimal WinOdd { get; set; }
        public decimal DrawOdd { get; set; }
        public decimal LossOdd { get; set; }

        public decimal GetOdd(int result)
        {
            if (result == 1)
                return WinOdd;
            else if (result == 2)
                return DrawOdd;
            else if (result == 3)
                return LossOdd;
            else
                return 0;
        }

        public override string DisplayName
        {
            get { return Local.DisplayName + " - " + Visitor.DisplayName; }
        }
    }
}
