using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRealm.Models
{
    class GameServer
    {
        public int id { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string key { get; set; }
        public int state { get; set; }

        public override string ToString()
        {
            return string.Format("{0};{1};0;1", id, state);
        }
    }
}
