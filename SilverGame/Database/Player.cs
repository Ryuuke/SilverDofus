using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SilverGame.Database
{
    class Player
    {
        public int CountPlayers()
        {
            int numberPlayers = 0;

            lock (DbManager._lock)
            {
                string req = "SELECT COUNT(*) FROM player";

                MySqlCommand command = new MySqlCommand(req, DbManager.connection);

                numberPlayers = (int)command.ExecuteScalar();
            }

            return numberPlayers;
        }
    }
}
