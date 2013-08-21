using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Items;

namespace SilverGame.Database.Repository
{
    class CharacterStatsRepository: Abstract.Repository
    {
        public static void Create(StatsManager stats)
        {
            const string query = "INSERT INTO character_stats SET Id=@Id, Vitality=@Vitality, Wisdom=@Wisdom," +
                                        "Strenght=@Strenght, Intelligence=@Intelligence, Chance=@Chance, Agility=@Agility";

            ExecuteQuery(query, GameDbManager.GetDatabaseConnection(),
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

        public static void Update(GeneralStats generalStats)
        {

        }

        public static void Remove(GeneralStats generalStats)
        {
            
        }
    }
}
