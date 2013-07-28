using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Gifts;
using SilverGame.Services;

namespace SilverGame.Database.Repository
{
    static class AccountRepository
    {
        public static void UpdateAccount(bool connected, int id = 0)
        {
            lock (RealmDbManager.Lock)
            {
                var query = id == 0
                    ? "UPDATE accounts SET connected = @connected"
                    : "UPDATE accounts SET connected = @connected WHERE id = @id";

                using (var command = new MySqlCommand(query, RealmDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@id", id));
                    command.Parameters.Add(new MySqlParameter("@connected", connected));

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
