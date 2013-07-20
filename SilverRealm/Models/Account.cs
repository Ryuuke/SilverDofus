using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRealm.Models
{
    class Account
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string pseudo { get; set; }
        public string question { get; set; }
        public string reponse { get; set; }
        public bool connected { get; set; }
        public int gmLevel { get; set; }
        public Nullable<DateTime> bannedUntil { get; set; }
        public Nullable<DateTime> subscription { get; set; }
    }
}
