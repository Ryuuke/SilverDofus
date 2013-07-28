using System;
using SilverGame.Database;
using SilverGame.Database.Connection;
using SilverGame.Services;

namespace SilverGame
{
    class Program
    {
        static void Main()
        {
            SilverConsole.Welcome();

            if (Config.LoadConfig() && GameDbManager.InitGameDatabse() && RealmDbManager.InitRealmDatabase())
            {
                Logs.LoadLogs();

                DatabaseProvider.LoadDatabase();

                var com = new Network.ToRealm.ToRealmClient();
                com.ConnectToRealm();
                
                var serv = new Network.Game.GameServer();
                serv.Run();
            }

            Console.ReadLine();
        }
    }
}
