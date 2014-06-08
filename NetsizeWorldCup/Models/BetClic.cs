using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetsizeWorldCup.Models.Betclic
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class sports
    {
        private Sport[] sportField;

        private System.DateTime file_dateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sport")]
        public Sport[] sport
        {
            get
            {
                return this.sportField;
            }
            set
            {
                this.sportField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime file_date
        {
            get
            {
                return this.file_dateField;
            }
            set
            {
                this.file_dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Sport
    {

        private Event[] eventField;

        private string nameField;

        private int idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("event")]
        public Event[] @event
        {
            get
            {
                return this.eventField;
            }
            set
            {
                this.eventField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Event
    {
        private Match[] matchField;

        private string nameField;

        private long idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("match")]
        public Match[] match
        {
            get
            {
                return this.matchField;
            }
            set
            {
                this.matchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Match
    {

        private Bet[] betsField;

        private string nameField;

        private uint idField;

        private System.DateTime start_dateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("bet", IsNullable = false)]
        public Bet[] bets
        {
            get
            {
                return this.betsField;
            }
            set
            {
                this.betsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime start_date
        {
            get
            {
                return this.start_dateField;
            }
            set
            {
                this.start_dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Bet
    {

        private BetChoice[] choiceField;

        private string codeField;

        private string nameField;

        private int idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("choice")]
        public BetChoice[] choice
        {
            get
            {
                return this.choiceField;
            }
            set
            {
                this.choiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BetChoice
    {

        private string nameField;

        private int idField;

        private decimal oddField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal odd
        {
            get
            {
                return this.oddField;
            }
            set
            {
                this.oddField = value;
            }
        }
    }
}