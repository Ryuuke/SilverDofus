using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SilverRealm.Services;
using SilverRealm.Models;

namespace SilverRealm.Database
{
    static class GameServerRepository
    {
        public static List<GameServer> GetAll()
        {
            var gameServers = new List<GameServer>();

            const string query = "SELECT * FROM gameservers";

            var command = new MySqlCommand(query, DbManager.Connection);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    gameServers.Add(new GameServer
                    {
                        Id = reader.GetInt16("id"),
                        Ip = reader.GetString("ip"),
                        Port = reader.GetInt16("port"),
                        ServerKey = reader.GetString("ServerKey"),
                        State = 0,
                    });
                }
                catch (Exception e)
                {
                    SilverConsole.WriteLine("SQL error : " + e.Message, ConsoleColor.Red);
                    Logs.LogWritter(Constant.ErrorsFolder, "SQL error : " + e.Message);
                }
            }

            reader.Close();

            if (gameServers.Any()) return gameServers;

            SilverConsole.WriteLine("Warning : Could not find Game Server On database", ConsoleColor.Yellow);
            Logs.LogWritter(Constant.RealmFolder, "Could not find Game Server On database");

            return gameServers;
        }
    }
}
