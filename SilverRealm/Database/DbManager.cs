﻿using System;
using MySql.Data.MySqlClient;
using Config = SilverRealm.Services.Config;

namespace SilverRealm.Database
{
    class DbManager
    {
        public static MySqlConnection Connection;
        public static Object Lock = new Object();

        public DbManager()
        {
            Connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}",
                                        Config.Get("Realm_Database_Host"),
                                        Config.Get("Realm_Database_Username"),
                                        Config.Get("Realm_Database_Password"),
                                        Config.Get("Realm_Database_Name")));
            Connection.Open();

            Console.WriteLine("Connection to database successfully");
        }
    }
}
