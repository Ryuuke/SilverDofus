using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;

namespace SilverGame.Database.Repository
{
    internal class AccountRepository : Abstract.Repository
    {
        public static void UpdateAccount(bool connected, int id = 0)
        {
            var query = id == 0
                ? "UPDATE accounts, characters SET accounts.connected = @connected WHERE characters.accountId = accounts.id"
                : "UPDATE accounts, characters SET accounts.connected = @connected WHERE accounts.id = @id AND characters.accountId = accounts.id";

            ExecuteQuery(query, RealmDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@id", id));
                    command.Parameters.Add(new MySqlParameter("@connected", connected));
                });
        }
    }
}
