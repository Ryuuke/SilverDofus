using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SilverRealm.Database
{
    class DbManager
    {
        public static MySqlConnection connection;
        public static Object _lock = new Object();

        public DbManager()
        {
            connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}",
                                        Services.Config.get("Realm_Database_Host"),
                                        Services.Config.get("Realm_Database_Username"),
                                        Services.Config.get("Realm_Database_Password"),
                                        Services.Config.get("Realm_Database_Name")));
            connection.Open();

            Console.WriteLine("Connection to database successfully");
        }
    }
}
