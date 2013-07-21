using MySql.Data.MySqlClient;

namespace SilverGame.Database
{
    class Player
    {
        public int CountPlayers()
        {
            int numberPlayers = 0;

            lock (DbManager.Lock)
            {
                const string req = "SELECT COUNT(*) FROM player";

                var command = new MySqlCommand(req, DbManager.Connection);

                numberPlayers = (int)command.ExecuteScalar();
            }

            return numberPlayers;
        }
    }
}
