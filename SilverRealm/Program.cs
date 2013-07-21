using System;

namespace SilverRealm
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.Config.LoadConfig();

            var server = new Network.Realm.RealmServer();

            var db = new Database.DbManager();

            var com = new Network.ToGame.ToGameServer();

            Console.ReadLine();
        }
    }
}
