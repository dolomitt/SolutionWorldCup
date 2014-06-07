using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace NetsizeWorldCup
{
    public class Phase : Entity
    {
        public string Name { get; set; }
        public int Rank { get; set; }

        public override string DisplayName
        {
            get { return Name; }
        }
    }
}
