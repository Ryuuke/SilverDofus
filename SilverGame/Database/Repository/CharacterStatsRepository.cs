using System;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Items;
using SilverGame.Services;

namespace SilverGame.Database.Repository
{
    class CharacterStatsRepository
    {
        public static void Create(int id)
        {
            var stats = new StatsManager { Id = id };

            lock (GameDbManager.Lock)
            {
                try
                {
                    const string query = "INSERT INTO character_stats SET Id=@Id, Vitality=@Vitality, Wisdom=@Wisdom," +
                                         "Strenght=@Strenght, Intelligence=@Intelligence, Chance=@Chance, Agility=@Agility";

                    using (var command = new MySqlCommand(query, GameDbManager.Connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@Id", stats.Id));
                        command.Parameters.Add(new MySqlParameter("@Vitality", stats.Vitality));
                        command.Parameters.Add(new MySqlParameter("@Wisdom", stats.Wisdom));
                        command.Parameters.Add(new MySqlParameter("@Strenght", stats.Strength));
                        command.Parameters.Add(new MySqlParameter("@Intelligence", stats.Intelligence));
                        command.Parameters.Add(new MySqlParameter("@Chance", stats.Chance));
                        command.Parameters.Add(new MySqlParameter("@Agility", stats.Agility));

                        command.ExecuteNonQuery();
                    }

                    lock (DatabaseProvider.StatsManager)
                        DatabaseProvider.StatsManager.Add(stats);
                }
                catch (Exception e)
                {
                    SilverConsole.WriteLine(e.Message, ConsoleColor.Red);
                }
            }
        }

        public static void Update(GeneralStats generalStats)
        {

        }

        public static void Remove(GeneralStats generalStats)
        {
            
        }
    }
}
