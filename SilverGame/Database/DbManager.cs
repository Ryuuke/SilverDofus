using System;
using MySql.Data.MySqlClient;

namespace SilverGame.Database
{
    class DbManager
    {
        public static MySqlConnection Connection;
        public static Object Lock = new Object();

        public DbManager()
        {
            Connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}",
                                        Services.Config.get("Game_Database_Host"),
                                        Services.Config.get("Game_Database_Username"),
                                        Services.Config.get("Game_Database_Password"),
                                        Services.Config.get("Game_Database_Name")));
            Connection.Open();

            Console.WriteLine("Connection to database successfully");
        }
    }
}
