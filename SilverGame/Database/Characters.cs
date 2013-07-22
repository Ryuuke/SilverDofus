using MySql.Data.MySqlClient;

namespace SilverGame.Database
{
    static class Characters
    {
        public static int CountPlayers()
        {
            int numberPlayers;

            lock (GameDbManager.Lock)
            {
                const string req = "SELECT COUNT(*) FROM characters";
                
                var command = new MySqlCommand(req, GameDbManager.Connection);

                numberPlayers = (int)command.ExecuteScalar();
            }

            return numberPlayers;
        }
    }
}
