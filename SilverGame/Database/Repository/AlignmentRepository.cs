using System;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Alignment;
using SilverGame.Services;

namespace SilverGame.Database.Repository
{
    class AlignmentRepository
    {
        public static void Create(int id)
        {
            var alignment = new Alignment{ Id = id };

            lock (GameDbManager.Lock)
            {
                try
                {
                    const string query = "INSERT INTO alignments SET Id=@Id, Type=@Type, Honor=@Honor," +
                                         "Deshonor=@Deshonor, Level=@Level, Grade=@Grade, Enabled=@Enabled";

                    using (var command = new MySqlCommand(query, GameDbManager.Connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@Id", alignment.Id));
                        command.Parameters.Add(new MySqlParameter("@Type", alignment.Type));
                        command.Parameters.Add(new MySqlParameter("@Honor", alignment.Honor));
                        command.Parameters.Add(new MySqlParameter("@Deshonor", alignment.Deshonor));
                        command.Parameters.Add(new MySqlParameter("@Level", alignment.Level));
                        command.Parameters.Add(new MySqlParameter("@Grade", alignment.Grade));
                        command.Parameters.Add(new MySqlParameter("@Enabled", alignment.Enabled));

                        command.ExecuteNonQuery();
                    }

                    lock (DatabaseProvider.Alignments)
                        DatabaseProvider.Alignments.Add(alignment);
                }
                catch (Exception e)
                {
                    SilverConsole.WriteLine(e.Message);
                }
            }
        }

        public static void Update(Alignment alignment)
        {

        }

        public static void Remove(Alignment alignment)
        {

        }
    }
}
