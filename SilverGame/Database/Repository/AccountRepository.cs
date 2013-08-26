using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;

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
    }
}
