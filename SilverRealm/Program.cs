using System;
using SilverRealm.Services;

namespace SilverRealm
{
    class Program
    {
        static void Main()
        {
            SilverConsole.Welcome();

            Services.Logs.LoadLogs();

            if (Services.Config.LoadConfig() && Database.DbManager.InitRealmDatabase())
            {
                Network.Realm.RealmClient.GameServers = Database.GameServerRepository.GetAll();

                var com = new Network.ToGame.ToGameServer();

                var server = new Network.Realm.RealmServer();
            }

            Console.ReadLine();
        }
    }
}
