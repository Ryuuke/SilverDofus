using System;
using MySql.Data.MySqlClient;
using SilverGame.Services;

namespace SilverGame.Database.Repository.Abstract
{
    abstract class Repository
    {
        protected static void ExecuteQuery(string query, MySqlConnection mySqlConnection, Action<MySqlCommand> paramAction)
        {
            try
            {
                using (mySqlConnection)
                {
                    using (var command = new MySqlCommand(query, mySqlConnection))
                    {
                        paramAction(command);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(string.Format("SQL Error : {0}", e.Message), ConsoleColor.Red);
            }
        }
    }
}
