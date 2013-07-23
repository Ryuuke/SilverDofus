using System;

namespace SilverRealm
{
    class Program
    {
        static void Main()
        {
            Services.Config.LoadConfig();

            Services.Logs.LoadLogs();

            var db = new Database.DbManager();

            Network.Realm.RealmClient.GameServers = Database.GameServerRepository.GetAll();

            var server = new Network.Realm.RealmServer();

            var com = new Network.ToGame.ToGameServer();

            Console.ReadLine();
        }
    }
}
