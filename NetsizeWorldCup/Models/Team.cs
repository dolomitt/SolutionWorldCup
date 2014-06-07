using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace NetsizeWorldCup
{
    public class Team : Entity
    {
        public string FlagUrl { get; set; }
        public string Name { get; set; }

        public virtual Group Group { get; set; }

        public override string DisplayName
        {
            get { return Name; }
        }
    }
}
