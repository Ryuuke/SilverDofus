using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.Config.loadConfig();

            Database.DbManager db = new Database.DbManager();

            Network.ToRealm.ToRealmClient com = new Network.ToRealm.ToRealmClient();

            Console.ReadLine();
        }
    }
}
