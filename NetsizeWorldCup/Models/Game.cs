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

        public int? Result { get; set; }

        public override string DisplayName
        {
            get { return Local.DisplayName + " - " + Visitor.DisplayName; }
        }
    }
}
