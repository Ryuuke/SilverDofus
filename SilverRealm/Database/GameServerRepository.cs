using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SilverRealm.Models;

namespace SilverRealm.Database
{
    static class GameServerRepository
    {
        public static List<GameServer> GetAll()
        {
            var gameServers = new List<GameServer>(); ;

            lock (DbManager.Lock)
            {
                const string req = "SELECT * FROM gameservers";

                var command = new MySqlCommand(req, DbManager.Connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        gameServers.Add(new GameServer()
                        {
                            Id = reader.GetInt16("id"),
                            Ip = reader.GetString("ip"),
                            Port = reader.GetInt32("port"),
                            Key = reader.GetString("key"),
                            State = reader.GetInt16("state"),
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("SQL error : " + e.Message);
                    }
                }

                reader.Close();

                if (!gameServers.Any())
                {
                    Console.WriteLine("Could not find Game Server On database");
                }
            }

            return gameServers;
        }
    }
}
