using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using SilverGame.Database.Connection;
using SilverGame.Models.Accounts;
using SilverGame.Models.Characters;
using SilverGame.Services;

namespace SilverGame.Database.Repository
{
    static class CharacterRepository
    {

        public static void CreateNewCharacter(string name, int classe, int sex, int color1, int color2, int color3, int accountId)
        {
            try
            {
                lock (GameDbManager.Lock)
                {
                    const string req =
                        "INSERT INTO characters SET name=@name, classe=@classe, sex=@sex, color1=@color1, color2=@color2," +
                        "color3=@color3, skin=@skin, level=@level";

                    using (var command = new MySqlCommand(req, GameDbManager.Connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@name", name));
                        command.Parameters.Add(new MySqlParameter("@classe", classe));
                        command.Parameters.Add(new MySqlParameter("@sex", sex));
                        command.Parameters.Add(new MySqlParameter("@color1", color1));
                        command.Parameters.Add(new MySqlParameter("@color2", color2));
                        command.Parameters.Add(new MySqlParameter("@color3", color3));
                        command.Parameters.Add(new MySqlParameter("@skin", int.Parse(classe + "" + sex)));
                        command.Parameters.Add(new MySqlParameter("@level", int.Parse(Config.Get("Starting_level"))));

                        command.ExecuteNonQuery();
                    }

                    Logs.LogWritter(Constant.GameFolder, string.Format("Création du personnage {0}", name));
                }
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(e.Message, ConsoleColor.Red);
                Logs.LogWritter(Constant.ErrorsFolder,
                    string.Format("Impossible de créer le personnage {0} : {1}", name, e.Message));
            }

            try
            {
                lock (RealmDbManager.Lock)
                {
                    const string req2 =
                        "INSERT INTO characters SET accountId=@accountId, gameserverId=(SELECT id FROM gameservers WHERE ServerKey=@key LIMIT 1), characterName=@name";

                    using (var command = new MySqlCommand(req2, RealmDbManager.Connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@accountId", accountId));
                        command.Parameters.Add(new MySqlParameter("@key", Config.Get("Game_key")));
                        command.Parameters.Add(new MySqlParameter("@name", name));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(e.Message, ConsoleColor.Red);
                Logs.LogWritter(Constant.ErrorsFolder,
                    string.Format(
                        "Impossible d'ajouter les informations sur le personnage {0} dans la table gameserver : {1}",
                        name, e.Message));
            }

            try
            {
                lock (DatabaseProvider.Characters)
                {
                    DatabaseProvider.Characters.Add(new Character
                    {
                        Id = DatabaseProvider.Characters.Count > 1
                            ? DatabaseProvider.Characters.OrderByDescending(x => x.Id).First().Id + 1
                            : 1,
                        Name = name,
                        Classe = classe,
                        Sex = sex,
                        Color1 = color1,
                        Color2 = color2,
                        Color3 = color3,
                        Level = int.Parse(Config.Get("Starting_level")),
                        Skin = int.Parse(classe + "" + sex),
                    });
                }

                lock (DatabaseProvider.CharactersAccount)
                    DatabaseProvider.CharactersAccount.Add(new CharactersAccount
                    {
                        Account = DatabaseProvider.Accounts.Find(x => x.Id == accountId),
                        Character = DatabaseProvider.Characters.Find(x => x.Name == name),
                    });
                
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(e.Message, ConsoleColor.Red);
            }
        }

        public static Character GetCharacter<T>(string column, T value)
        {
            Character character = null;

            lock (GameDbManager.Lock)
            {
                var req = "SELECT * FROM characters WHERE " + column + "=@value";

                using (var command = new MySqlCommand(req, GameDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@value", value));

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            character = new Character
                            {
                                Id = reader.GetInt16("id"),
                                Name = reader.GetString("name"),
                                Classe = reader.GetInt16("classe"),
                                Color1 = reader.GetInt32("color1"),
                                Color2 = reader.GetInt32("color2"),
                                Color3 = reader.GetInt32("color3"),
                                Level = reader.GetInt16("level"),
                                Sex = reader.GetInt16("sex"),
                                Skin = reader.GetInt16("skin"),
                            };
                        }
                        catch (Exception e)
                        {
                            SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
                            Logs.LogWritter(Constant.ErrorsFolder,
                                string.Format(
                                    "SQL Error : Impossible de charger le compte qui a pour colonne {0} {1} : {2}", column,
                                    value, e.Message));
                        }
                    }

                    reader.Close();
                }
            }

            return character;
        }
    }
}