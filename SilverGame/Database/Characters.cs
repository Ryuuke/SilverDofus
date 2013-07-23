using System;
using MySql.Data.MySqlClient;
using SilverGame.Services;

namespace SilverGame.Database
{
    static class Characters
    {
        public static int CountPlayers()
        {
            int numberPlayers;

            lock (GameDbManager.Lock)
            {
                const string req = "SELECT COUNT(*) FROM characters";
                
                var command = new MySqlCommand(req, GameDbManager.Connection);

                numberPlayers = (int)command.ExecuteScalar();
            }

            return numberPlayers;
        }

        public static bool Exist(string charName)
        {
            lock (GameDbManager.Lock)
            {
                const string req = "SELECT COUNT(*) FROM characters WHERE name=@name";

                var command = new MySqlCommand(req, GameDbManager.Connection);

                command.Parameters.Add(new MySqlParameter("@name",charName));

                return (command.ExecuteScalar().Equals(1));
            }
        }

        public static void CreateNewCharacter(string name, int classe, int sex, int color1, int color2, int color3, int accountId)
        {
            try
            {
                lock (GameDbManager.Lock)
                {
                    const string req =
                        "INSERT INTO characters SET name=@name, classe=@classe, sex=@sex, color1=@color1, color2=@color2," +
                        "color3=@color3, skin=@skin, level=@level";

                    var command = new MySqlCommand(req, GameDbManager.Connection);

                    command.Parameters.Add(new MySqlParameter("@name", name));
                    command.Parameters.Add(new MySqlParameter("@classe", classe));
                    command.Parameters.Add(new MySqlParameter("@sex", sex));
                    command.Parameters.Add(new MySqlParameter("@color1", color1));
                    command.Parameters.Add(new MySqlParameter("@color2", color2));
                    command.Parameters.Add(new MySqlParameter("@color3", color3));
                    command.Parameters.Add(new MySqlParameter("@skin", int.Parse(classe + "" + sex)));
                    command.Parameters.Add(new MySqlParameter("@level", int.Parse(Config.Get("Starting_level"))));

                    command.ExecuteNonQuery();

                    Logs.LogWritter(Constant.GameFolder, string.Format("Création du personnage {0}", name));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logs.LogWritter(Constant.ErrorsFolder, string.Format("Impossible de créer le personnage {0} : {1}", name, e.Message));
            }

            try
            {
                lock (RealmDbManager.Lock)
                {
                    const string req2 = "INSERT INTO characters SET accountId=@accountId, gameserverId=(SELECT id FROM gameservers WHERE ServerKey=@key LIMIT 1), characterName=@name";

                    var command = new MySqlCommand(req2, RealmDbManager.Connection);

                    command.Parameters.Add(new MySqlParameter("@accountId", accountId));
                    command.Parameters.Add(new MySqlParameter("@key", Config.Get("Game_key")));
                    command.Parameters.Add(new MySqlParameter("@name", name));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logs.LogWritter(Constant.ErrorsFolder, string.Format("Impossible d'ajouter les informations sur le personnage {0} a la table gameserver : {1}", name, e.Message));
            }
        }
    }
}
