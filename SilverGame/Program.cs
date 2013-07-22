using System;

namespace SilverGame
{
    class Program
    {
        static void Main()
        {
            Services.Config.LoadConfig();

            var db = new Database.GameDbManager();

            var com = new Network.ToRealm.ToRealmClient();

            var serv = new Network.Game.GameServer();

            Console.ReadLine();
        }
    }
}
