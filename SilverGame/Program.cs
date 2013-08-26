using System;
using System.Reflection;
using System.Transactions;
using SilverGame.Database;
using SilverGame.Database.Connection;
using SilverGame.Services;

namespace SilverGame
{
    static class Program
    {
        static void Main()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;

            SilverConsole.Welcome();

            if (Config.LoadConfig() && GameDbManager.TestConnectivityToGameDb() && RealmDbManager.TestConnectivityToRealmDb())
            {
                Logs.LoadLogs();

                SilverConsole.LoadTimer();

                DatabaseProvider.LoadDatabase();

                var com = new Network.ToRealm.ToRealmClient();
                com.ConnectToRealm();
                
                var serv = new Network.Game.GameServer();
                serv.Run();
            }

            Console.Read();
        }
    }
}
