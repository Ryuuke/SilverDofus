using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Alignment;

namespace SilverGame.Database.Repository
{
    static class AlignmentRepository
    {
        public static void Create(Alignment alignment)
        {
            const string query = "INSERT INTO alignments SET Id=@Id, Type=@Type, Honor=@Honor," +
                                 "Deshonor=@Deshonor, Level=@Level, Grade=@Grade, Enabled=@Enabled";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@Id", alignment.Id));
                    command.Parameters.Add(new MySqlParameter("@Type", alignment.Type));
                    command.Parameters.Add(new MySqlParameter("@Honor", alignment.Honor));
                    command.Parameters.Add(new MySqlParameter("@Deshonor", alignment.Deshonor));
                    command.Parameters.Add(new MySqlParameter("@Level", alignment.Level));
                    command.Parameters.Add(new MySqlParameter("@Grade", alignment.Grade));
                    command.Parameters.Add(new MySqlParameter("@Enabled", alignment.Enabled));
                });

            lock (DatabaseProvider.Alignments)
                DatabaseProvider.Alignments.Add(alignment);
        }

        public static void Update(Alignment alignment)
        {
            const string query = "UPDATE alignments SET Type=@Type, Honor=@Honor," +
                                 "Deshonor=@Deshonor, Level=@Level, Grade=@Grade, Enabled=@Enabled " +
                                 "WHERE id=@Id";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@Id", alignment.Id));
                    command.Parameters.Add(new MySqlParameter("@Type", alignment.Type));
                    command.Parameters.Add(new MySqlParameter("@Honor", alignment.Honor));
                    command.Parameters.Add(new MySqlParameter("@Deshonor", alignment.Deshonor));
                    command.Parameters.Add(new MySqlParameter("@Level", alignment.Level));
                    command.Parameters.Add(new MySqlParameter("@Grade", alignment.Grade));
                    command.Parameters.Add(new MySqlParameter("@Enabled", alignment.Enabled));
                });
        }

        public static void Remove(Alignment alignment)
        {

        }
    }
}
