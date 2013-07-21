using System;
using MySql.Data.MySqlClient;
using SilverRealm.Models;

namespace SilverRealm.Database
{
    static class AccountRepository
    {
        public static Account GetAccount(string username)
        {
            Account account = null;

            lock (DbManager.Lock)
            {
                const string req = "SELECT * FROM accounts WHERE username=@username";

                var command = new MySqlCommand(req, DbManager.Connection); 
                
                command.Parameters.Add(new MySqlParameter("@username", username));
                    
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
                            BannedUntil = Convert.IsDBNull(reader["bannedUntil"]) ? (DateTime?)null : reader.GetDateTime("bannedUntil"),
                            Subscription = Convert.IsDBNull(reader["subscription"]) ? (DateTime?)null : reader.GetDateTime("subscription"),
                        };
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("SQL Error " + e.Message);
                    }
                }

                reader.Close();
            }

            return account;
        }
    }
}
