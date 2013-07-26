using System;
using MySql.Data.MySqlClient;
using SilverRealm.Services;
using Config = SilverRealm.Services.Config;

namespace SilverRealm.Database
{
    class DbManager
    {
        public static MySqlConnection Connection;
        public static Object Lock = new Object();

        public static bool InitRealmDatabase()
        {
            Connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}",
                                        Config.Get("Realm_Database_Host"),
                                        Config.Get("Realm_Database_Username"),
                                        Config.Get("Realm_Database_Password"),
                                        Config.Get("Realm_Database_Name")));
            try
            {
                SilverConsole.WriteLine("Connection to Realm Database...", ConsoleColor.DarkGreen);

                Connection.Open();

                SilverConsole.WriteLine("SQL : Connection to Realm database successfully", ConsoleColor.Green);

                return true;
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
                Logs.LogWritter(Constant.ErrorsFolder, "Realm connection database SQL error : " + e.Message);

                return false;
            }
        }
    }
}
