using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRealm
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.Config.loadConfig();

            Network.Realm.RealmServer server = new Network.Realm.RealmServer();

            Database.DbManager db = new Database.DbManager();

            Network.ToGame.ToGameServer com = new Network.ToGame.ToGameServer();

            Console.ReadLine();
        }
    }
}
