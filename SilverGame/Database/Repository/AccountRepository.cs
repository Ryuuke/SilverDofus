using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Items;

namespace SilverGame.Database.Repository
{
    static class AccountRepository
    {
        public static void UpdateAccount(int id = 0, bool connected = true)
        {
            if (id != 0 && connected)
            {
                const string query = "INSERT INTO gameconnection SET connected = 1, accountId=@accountid, gameserverid=@gsId";

                Base.Repository.ExecuteQuery(query, RealmDbManager.GetDatabaseConnection(),
                    (command) =>
                    {
                        command.Parameters.Add(new MySqlParameter("@accountid", id));
                        command.Parameters.Add(new MySqlParameter("@gsId", DatabaseProvider.ServerId));
                    });
            }
            else if (connected == false && id == 0)
            {
                const string query = "DELETE FROM gameconnection";

                Base.Repository.ExecuteQuery(query, RealmDbManager.GetDatabaseConnection(),
                    (command) => { });
            }
            else if (connected == false && id != 0)
            {
                const string query = "DELETE FROM gameconnection WHERE accountid=@accountid AND gameserverid=@gsId";

                Base.Repository.ExecuteQuery(query, RealmDbManager.GetDatabaseConnection(),
                    (command) =>
                    {
                        command.Parameters.Add(new MySqlParameter("@accountid", id));
                        command.Parameters.Add(new MySqlParameter("@gsId", DatabaseProvider.ServerId));
                    });
            }
        }

        public static void UpdateCharactersList(int id)
        {
            const string query = "SELECT * FROM characters WHERE accountId = @accountId AND gameServerId = @gsId";

            var charactersId = new List<int>();
            
            using (var connection = RealmDbManager.GetDatabaseConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add(new MySqlParameter("@accountId", id));
                    command.Parameters.Add(new MySqlParameter("@gsId", DatabaseProvider.ServerId));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            charactersId.Add(reader.GetInt16("characterId"));
                        }
                    }
                }
            }

            const string query2 = "SELECT * FROM characters WHERE FIND_IN_SET(id, @charactersId)";

            using (var connection = GameDbManager.GetDatabaseConnection())
            {
                using (var command = new MySqlCommand(query2, connection))
                {
                    command.Parameters.Add(new MySqlParameter("@charactersId", string.Join(",", charactersId)));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var character = DatabaseProvider.Characters.Find(x => x.Id == reader.GetInt16("id"));

                            character.Name = reader.GetString("name");
                            character.Color1 = reader.GetInt32("color1");
                            character.Color2 = reader.GetInt32("color2");
                            character.Color3 = reader.GetInt32("color3");
                            character.Kamas = reader.GetInt32("kamas");
                            character.Level = reader.GetInt16("level");
                            character.Sex = reader.GetInt16("sex");
                            character.Skin = reader.GetInt16("skin");
                            character.Map = DatabaseProvider.Maps.Find(x => x.Id == reader.GetInt32("mapId"));
                            character.MapCell = reader.GetInt32("cellId");
                            character.Direction = reader.GetInt16("direction");
                            character.StatsPoints = reader.GetInt32("statsPoints");
                            character.SpellPoints = reader.GetInt32("spellsPoints");
                        }
                    }
                }
            }
        }
    }
}
