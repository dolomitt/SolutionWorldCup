using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using NetsizeWorldCup.Models;

namespace NetsizeWorldCup
{
    public abstract class Entity : IEntity
    {
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }
        public abstract string DisplayName { get; }

        public Entity()
        {
            CreationDate = DateTime.UtcNow;
        }
    }

    public abstract class Ownership : Entity 
    {
        public virtual ApplicationUser Owner { get; set; }
    }
}
