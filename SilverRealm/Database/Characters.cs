using MySql.Data.MySqlClient;

namespace SilverRealm.Database
{
    internal static class Characters
    {
        public static string GetCharactersByGameServer(int accountId, string format)
        {
            var charactersByGameServer = string.Empty;

            const string query =
                "Select gameServerId, count(characterId) AS numberCharacters FROM characters WHERE accountId=@accountId GROUP by gameServerId;";

            lock (DbManager.Lock)
            {
                using (var connection = DbManager.GetConnection())
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@accountId", accountId));

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                            charactersByGameServer = string.Concat(charactersByGameServer,
                                string.Format(format, reader.GetInt16("gameServerId"), reader.GetInt16("numberCharacters")));

                        reader.Close();

                        return charactersByGameServer;
                    }
                }
            }
        }
    }
}

