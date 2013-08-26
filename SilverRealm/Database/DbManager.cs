using System;
using MySql.Data.MySqlClient;
using SilverRealm.Services;
using Config = SilverRealm.Services.Config;

namespace SilverRealm.Database
{
   static class DbManager
    {
        private static readonly string ConnectionString = string.Format("server={0};uid={1};pwd={2};database={3}",
            Config.Get("Realm_Database_Host"),
            Config.Get("Realm_Database_Username"),
            Config.Get("Realm_Database_Password"),
            Config.Get("Realm_Database_Name"));

        public static Object Lock = new Object();

        public static MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(ConnectionString);

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
                Logs.LogWritter(Constant.ErrorsFolder, "Realm connection database SQL error : " + e.Message);
            }

            return connection;
        }

        public static bool TestConnectivityToRealmDb()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    SilverConsole.WriteLine("Connected To Realm Database Successfuly", ConsoleColor.Green);

                    Logs.LogWritter(Constant.RealmFolder, "Connected To Realm Database Successfuly");

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
}
