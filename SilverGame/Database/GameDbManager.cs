using System;
using MySql.Data.MySqlClient;
using Config = SilverGame.Services.Config;

namespace SilverGame.Database
{
    class GameDbManager
    {
        public static MySqlConnection Connection;
        public static Object Lock = new Object();

        public GameDbManager()
        {
            Connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}",
                                        Config.Get("Game_Database_Host"),
                                        Config.Get("Game_Database_Username"),
                                        Config.Get("Game_Database_Password"),
                                        Config.Get("Game_Database_Name")));
            Connection.Open();
        }
    }
}
