using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SilverRealm.Models;

namespace SilverRealm.Database
{
    static class Characters
    {
        public static Dictionary<int, int> getCharactersByGameServer(int accountId)
        {
            Dictionary<int, int> charactersByGameServer = new Dictionary<int, int>();

            lock (DbManager._lock)
            {
                string req = "Select gameServerId, count(characterId) AS numberCharacters FROM characters WHERE accountId=@accountId GROUP by gameServerId;";

                MySqlCommand command = new MySqlCommand(req, DbManager.connection);

                command.Parameters.Add(new MySqlParameter("@accountId", accountId));

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                    charactersByGameServer.Add(reader.GetInt16("gameServerId"), reader.GetInt16("numberCharacters"));

                reader.Close();
            }

            return charactersByGameServer;
        }
    }
}
