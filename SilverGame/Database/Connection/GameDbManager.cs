using System;
using MySql.Data.MySqlClient;
using SilverGame.Services;

namespace SilverGame.Database.Connection
{
    internal static class GameDbManager
    {
        private static readonly string ConnectionString = string.Format("server={0};uid={1};pwd={2};database={3}",
            Config.Get("Game_Database_Host"),
            Config.Get("Game_Database_Username"),
            Config.Get("Game_Database_Password"),
            Config.Get("Game_Database_Name"));

        public static MySqlConnection GetDatabaseConnection()
        {
            var connection = new MySqlConnection(ConnectionString);

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
                Logs.LogWritter(Constant.ErrorsFolder, "Game connection database SQL error : " + e.Message);
            }

            return connection;
        }

        public static bool TestConnectivityToGameDb()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    SilverConsole.WriteLine("Connected To Game Database Successfuly", ConsoleColor.Green);
                    Logs.LogWritter(Constant.GameFolder, "Connected To Game Database Successfuly");

                    return true;
                }
                catch (Exception e)
                {
                    SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
                    Logs.LogWritter(Constant.ErrorsFolder, "Game connection database SQL error : " + e.Message);
                    return false;
                }
            }
        }
    }
}