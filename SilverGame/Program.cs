using System;
using SilverGame.Services;

namespace SilverGame
{
    class Program
    {
        static void Main()
        {
            Services.Config.LoadConfig();

            Logs.LoadLogs();

            var db = new Database.GameDbManager();

            var dbRealm = new Database.RealmDbManager();

            var com = new Network.ToRealm.ToRealmClient();

            var serv = new Network.Game.GameServer();

            Console.ReadLine();
        }
    }
}
