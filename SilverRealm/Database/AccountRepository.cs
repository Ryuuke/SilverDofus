using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SilverRealm.Services;
using SilverRealm.Models;
using Constant = SilverRealm.Services.Constant;

namespace SilverRealm.Database
{
    static class AccountRepository
    {
        public static Account GetAccount<T>(string column, T attribut)
        {
            if (!column.Equals(Constant.UsernameColumnName) && !column.Equals(Constant.PseudoColumnName))
                return null;

            Account account = null;

            var query = "SELECT * FROM accounts WHERE " + column + "=@attribut";

            lock (DbManager.Lock)
            {
                using (var connection = DbManager.GetConnection())
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@attribut", attribut));

                        var reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            try
                            {
                                account = new Account
                                {
                                    Id = reader.GetInt16("id"),
                                    Username = reader.GetString("username"),
                                    Password = reader.GetString("pass"),
                                    Pseudo = reader.GetString("pseudo"),
                                    Question = reader.GetString("question"),
                                    Reponse = reader.GetString("reponse"),
                                    Connected = GetConnection(reader.GetInt16("id")),
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
            }
            

            return account;
        }

        public static void UpdateAccount(int gsId)
        {
            const string query = "DELETE FROM gameconnection WHERE gameServerId=@gsId";

            lock (DbManager.Lock)
            {
                using (var connection = DbManager.GetConnection())
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@gsId", gsId));

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static bool GetConnection(int accountId)
        {
            var connected = new List<bool>();

            const string query = "SELECT connected FROM gameconnection WHERE accountId=@accountId";

            using (var connection = DbManager.GetConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Add(new MySqlParameter("@accountId", accountId));

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        connected.Add(reader.GetBoolean("connected"));
                    }

                    reader.Close();
                }
            }

            return (connected.Any(x => x));
        }
    }
}
