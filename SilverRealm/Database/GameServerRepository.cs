using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SilverRealm.Models;

namespace SilverRealm.Database
{
    static class GameServerRepository
    {
        public static List<GameServer> getAll()
        {
            List<GameServer> gameServers = new List<GameServer>(); ;

            lock (DbManager._lock)
            {
                string req = "SELECT * FROM gameservers";

                MySqlCommand command = new MySqlCommand(req, DbManager.connection);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        gameServers.Add(new GameServer()
                        {
                            id = reader.GetInt16("id"),
                            ip = reader.GetString("ip"),
                            port = reader.GetInt32("port"),
                            key = reader.GetString("key"),
                            state = reader.GetInt16("state"),
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
