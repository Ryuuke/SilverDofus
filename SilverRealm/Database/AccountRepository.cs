using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SilverRealm.Models;

namespace SilverRealm.Database
{
    static class AccountRepository
    {
        public static Account getAccount(string username)
        {
            Account account = null;

            lock (DbManager._lock)
            {
                string req = "SELECT * FROM accounts WHERE username=@username";

                MySqlCommand command = new MySqlCommand(req, DbManager.connection);
                
                
                command.Parameters.Add(new MySqlParameter("@username", username));
                    
                MySqlDataReader reader = command.ExecuteReader();
 
                if (reader.Read())
                {
                    try
                    {
                        account = new Account()
                        {
                            id = reader.GetInt32("id"),
                            username = reader.GetString("username"),
                            password = reader.GetString("pass"),
                            pseudo = reader.GetString("pseudo"),
                            question = reader.GetString("question"),
                            reponse = reader.GetString("reponse"),
                            connected = reader.GetBoolean("connected"),
                            gmLevel = reader.GetInt16("gmLevel"),
                            bannedUntil = Convert.IsDBNull(reader["bannedUntil"]) ? (Nullable<DateTime>)null : reader.GetDateTime("bannedUntil"),
                            subscription = Convert.IsDBNull(reader["subscription"]) ? (Nullable<DateTime>)null : reader.GetDateTime("subscription"),
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
