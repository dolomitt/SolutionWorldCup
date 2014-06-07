using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using NetsizeWorldCup.Models;

namespace NetsizeWorldCup
{
    public class Message : Ownership
    {
        public string Body { get; set; }

        public override string DisplayName
        {
            get { return Body; }
        }
    }
}
