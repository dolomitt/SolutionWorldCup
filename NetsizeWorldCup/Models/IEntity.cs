using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace NetsizeWorldCup
{
    public interface IEntity
    {
        int ID { get; set; }
        DateTime CreationDate { get; set; }
        string DisplayName { get; }
    }
}
