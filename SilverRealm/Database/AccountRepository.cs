using System;
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

            var command = new MySqlCommand(query, DbManager.Connection);

            command.Parameters.Add(new MySqlParameter("@attribut", attribut));

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
                                ? (DateTime?) null
                                : reader.GetDateTime("bannedUntil"),
                        Subscription =
                            Convert.IsDBNull(reader["subscription"])
                                ? (DateTime?) null
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

            return account;
        }
    }
}
