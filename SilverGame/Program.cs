using System;
using SilverGame.Services;

namespace SilverGame
{
    class Program
    {
        static void Main()
        {
            SilverConsole.Welcome();

            if (Services.Config.LoadConfig() && Database.GameDbManager.InitGameDatabse() && Database.RealmDbManager.InitRealmDatabase())
            {
                Logs.LoadLogs();

                var com = new Network.ToRealm.ToRealmClient();

                var serv = new Network.Game.GameServer();
            }

            Console.ReadLine();
        }
    }
}
