using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models;
using SilverGame.Models.Accounts;
using SilverGame.Services;

namespace SilverGame.Database.Repository
{
    static class AccountRepository
    {
        public static Account GetAccount<T>(string column, T value)
        {
            Account account = null;

            lock (RealmDbManager.Lock)
            {
                var req = "SELECT * FROM accounts WHERE " + column + "=@value";

                using (var command = new MySqlCommand(req, RealmDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@value", value));

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            account = new Account
                            {
                                Id = reader.GetInt32("id"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("pass"),
                                Pseudo = reader.GetString("pseudo"),
                                Question = reader.GetString("question"),
                                Reponse = reader.GetString("reponse"),
                                Connected = reader.GetBoolean("connected"),
                                GmLevel = reader.GetInt16("gmLevel"),
                                BannedUntil =
                                    Convert.IsDBNull(reader["bannedUntil"])
                                        ? (DateTime?)null
                                        : reader.GetDateTime("bannedUntil"),
                                Subscription =
                                    Convert.IsDBNull(reader["subscription"])
                                        ? (DateTime?)null
                                        : reader.GetDateTime("subscription"),
                            };
                        }
                        catch (Exception e)
                        {
                            SilverConsole.WriteLine("SQL Error " + e.Message, ConsoleColor.Red);
                            Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
                        }
                    }

                    reader.Close();
                }
            }

            return account;
        }

        public static void UpdateAccount(bool connected, int id = 0)
        {
            lock (RealmDbManager.Lock)
            {
                var req = id == 0 ? "UPDATE accounts SET connected = @connected" : "UPDATE accounts SET connected = @connected WHERE id = @id";

                using (var command = new MySqlCommand(req, RealmDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@id", id));
                    command.Parameters.Add(new MySqlParameter("@connected", connected));

                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Gift> GetGifts(int id)
        {
            var gifts = new List<Gift>();

            lock (GameDbManager.Lock)
            {
                const string req = "SELECT * FROM account_gifts WHERE accountId=@id";

                using (var command = new MySqlCommand(req, GameDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@id", id));

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                    }

                    reader.Close();
                }
            }

            return gifts;
        }
    }
}
