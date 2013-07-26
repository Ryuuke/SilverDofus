using System;
using SilverGame.Database;
using SilverGame.Database.Connection;
using SilverGame.Database.Repository;
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

                SilverConsole.WriteLine("loading database resources...", ConsoleColor.DarkGreen);

                DatabaseProvider.LoadDatabase();

                var com = new Network.ToRealm.ToRealmClient();
                var serv = new Network.Game.GameServer();
            }

            Console.ReadLine();
        }
    }
}
