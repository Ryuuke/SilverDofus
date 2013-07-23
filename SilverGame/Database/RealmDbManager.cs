using System;
using MySql.Data.MySqlClient;
using SilverGame.Services;

namespace SilverGame.Database
{
    class RealmDbManager
    {
        public static MySqlConnection Connection;
        public static Object Lock = new Object();

        public RealmDbManager()
        {
            Connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}",
                                        Config.Get("Realm_Database_Host"),
                                        Config.Get("Realm_Database_Username"),
                                        Config.Get("Realm_Database_Password"),
                                        Config.Get("Realm_Database_Name")));
            try
            {
                Connection.Open();

                Console.WriteLine("Connection to Realm database successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("SQL Error : " + e.Message);
                Logs.LogWritter(Constant.ErrorsFolder, "Realm connection database SQL error : " + e.Message);
            }
        }
    }
}
