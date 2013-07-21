﻿using MySql.Data.MySqlClient;

namespace SilverRealm.Database
{
    static class Characters
    {
        public static string GetCharactersByGameServer(int accountId)
        {
            var charactersByGameServer = string.Empty;

            lock (DbManager.Lock)
            {
                const string req = "Select gameServerId, count(characterId) AS numberCharacters FROM characters WHERE accountId=@accountId GROUP by gameServerId;";

                var command = new MySqlCommand(req, DbManager.Connection);

                command.Parameters.Add(new MySqlParameter("@accountId", accountId));

                var reader = command.ExecuteReader();

                while (reader.Read())
                    charactersByGameServer = string.Concat(charactersByGameServer, string.Format("|{0},{1}",reader.GetInt16("gameServerId"), reader.GetInt16("numberCharacters")));

                reader.Close();
            }

            return charactersByGameServer;
        }
    }
}
