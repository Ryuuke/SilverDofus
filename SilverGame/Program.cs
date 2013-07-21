using System;

namespace SilverGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.Config.LoadConfig();

            var db = new Database.DbManager();

            var com = new Network.ToRealm.ToRealmClient();

            Console.ReadLine();
        }
    }
}
