using System;
using System.Reflection;
using SilverRealm.Services;

namespace SilverRealm
{
    static class Program
    {
        static void Main()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;

            SilverConsole.Welcome();

            Logs.LoadLogs();

            if (Config.LoadConfig() && Database.DbManager.TestConnectivityToRealmDb())
            {
                Network.Realm.RealmClient.GameServers = Database.GameServerRepository.GetAll();

                var com = new Network.ToGame.ToGameServer();
                com.Run();

                var server = new Network.Realm.RealmServer();
                server.Run();
            }

            Console.Read();
        }
    }
}
