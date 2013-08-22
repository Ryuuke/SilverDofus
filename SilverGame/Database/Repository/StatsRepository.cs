using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Items;

namespace SilverGame.Database.Repository
{
    static class StatsRepository
    {
        public static void Create(StatsManager stats)
        {
            const string query = "INSERT INTO character_stats SET Id=@Id, Vitality=@Vitality, Wisdom=@Wisdom," +
                                        "Strenght=@Strenght, Intelligence=@Intelligence, Chance=@Chance, Agility=@Agility";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@Id", stats.Id));
                    command.Parameters.Add(new MySqlParameter("@Vitality", stats.Vitality));
                    command.Parameters.Add(new MySqlParameter("@Wisdom", stats.Wisdom));
                    command.Parameters.Add(new MySqlParameter("@Strenght", stats.Strength));
                    command.Parameters.Add(new MySqlParameter("@Intelligence", stats.Intelligence));
                    command.Parameters.Add(new MySqlParameter("@Chance", stats.Chance));
                    command.Parameters.Add(new MySqlParameter("@Agility", stats.Agility));
                });

                lock (DatabaseProvider.StatsManager)
                    DatabaseProvider.StatsManager.Add(stats);
        }

        public static void Update(StatsManager stats)
        {
            const string query = "UPDATE character_stats SET Vitality=@Vitality, Wisdom=@Wisdom," +
                                        "Strenght=@Strenght, Intelligence=@Intelligence, Chance=@Chance, Agility=@Agility " +
                                 "WHERE id=@Id";

            Base.Repository.ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
                (command) =>
                {
                    command.Parameters.Add(new MySqlParameter("@Id", stats.Id));
                    command.Parameters.Add(new MySqlParameter("@Vitality", stats.Vitality));
                    command.Parameters.Add(new MySqlParameter("@Wisdom", stats.Wisdom));
                    command.Parameters.Add(new MySqlParameter("@Strenght", stats.Strength));
                    command.Parameters.Add(new MySqlParameter("@Intelligence", stats.Intelligence));
                    command.Parameters.Add(new MySqlParameter("@Chance", stats.Chance));
                    command.Parameters.Add(new MySqlParameter("@Agility", stats.Agility));
                });
        }

        public static void Remove(StatsManager stats)
        {
            
        }
    }
}
