using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Database.Connection;
using SilverGame.Database.Repository;
using SilverGame.Models;
using SilverGame.Models.Accounts;
using MySql.Data.MySqlClient;
using SilverGame.Models.Characters;
using SilverGame.Models.Items;
using SilverGame.Services;

namespace SilverGame.Database
{
    class DatabaseProvider
    {
        public static readonly List<Account> Accounts = new List<Account>();
        public static readonly List<CharactersAccount> CharactersAccount = new List<CharactersAccount>();
        public static readonly List<Character> Characters = new List<Character>();
        public static readonly List<InventoryItem> InventoryItems = new List<InventoryItem>();
        public static readonly List<ItemInfos> ItemsInfos = new List<ItemInfos>();
        public static int ServerId; 

        public static void LoadDatabase()
        {
            LoadServerId();
            LoadAccounts();
            LoadCharactersAccount();
            LoadCharacters();
            LoadItemInfos();
            LoadInventoryItems();
        }

        private static void LoadServerId()
        {
            lock (RealmDbManager.Lock)
            {
                const string req = "SELECT id FROM gameservers WHERE ServerKey=@key LIMIT 1";

                using (var command = new MySqlCommand(req, RealmDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@key", Config.Get("Game_key")));

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                        ServerId = reader.GetInt16("id");

                    reader.Close();
                }
            }
        }

        public static void LoadAccounts()
        {
            const string req = "SELECT * FROM accounts";

            using (var command = new MySqlCommand(req, RealmDbManager.Connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        var account = new Account
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
                                    ? (DateTime?)null
                                    : reader.GetDateTime("bannedUntil"),
                            Subscription =
                                Convert.IsDBNull(reader["subscription"])
                                    ? (DateTime?)null
                                    : reader.GetDateTime("subscription"),
                        };

                        lock (Accounts)
                            Accounts.Add(account);
                    }
                    catch (Exception e)
                    {
                        SilverConsole.WriteLine("SQL Error " + e.Message, ConsoleColor.Red);
                        Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
                    }
                }

                reader.Close();
            }

            SilverConsole.WriteLine(String.Format("loaded {0} accounts successfully", Accounts.Count), ConsoleColor.Green);
        }

        public static void LoadCharactersAccount()
        {
            lock (RealmDbManager.Lock)
            {
                var tableResult = new List<KeyValuePair<int, string>>();

                const string req = "SELECT * FROM characters WHERE gameserverId =@ServerId";

                using (var command = new MySqlCommand(req, RealmDbManager.Connection))
                {
                    command.Parameters.Add(new MySqlParameter("@ServerId", ServerId));

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        tableResult.Add(new KeyValuePair<int, string>(
                                reader.GetInt16("AccountId"),
                                reader.GetString("characterName")
                            ));
                    }

                    reader.Close();

                    foreach (var charactersAccount in tableResult.Select(keyValuePair => new CharactersAccount
                    {
                        Account = AccountRepository.GetAccount(Constant.AccountIdColumnName, keyValuePair.Key),
                        Character = CharacterRepository.GetCharacter(Constant.CharacterNameColumnName, keyValuePair.Value),
                    }))
                    {
                        lock (CharactersAccount)
                            CharactersAccount.Add(charactersAccount);
                    }
                }
            }
        }

        public static void LoadCharacters()
        {
            const string req = "SELECT * FROM Characters";

            using (var command = new MySqlCommand(req, GameDbManager.Connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        var character = new Character
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

                        lock (Characters)
                            Characters.Add(character);
                    }
                    catch (Exception e)
                    {
                        SilverConsole.WriteLine(String.Format("SQL Error : {0}", e.Message), ConsoleColor.Red);
                    }
                }

                reader.Close();
            }

            SilverConsole.WriteLine(String.Format("loaded {0} Characters successfully", Characters.Count), ConsoleColor.Green);
        }

        public static void LoadItemInfos()
        {
            const string req = "SELECT * FROM items_infos";

            using (var command = new MySqlCommand(req, GameDbManager.Connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var itemInfos = new ItemInfos
                    {
                        Id = reader.GetInt16("ID"),
                        Name = reader.GetString("Name"),
                        Type = reader.GetInt16("Type"),
                        Level = reader.GetInt16("Level"),
                        Weight = reader.GetInt16("Weight"),
                        WeaponInfo = reader.GetString("WeaponInfo"),
                        TwoHands = reader.GetBoolean("TwoHands"),
                        IsEthereal = reader.GetBoolean("IsEthereal"),
                        IsBuff = reader.GetBoolean("IsBuff"),
                        Usable = reader.GetBoolean("Usable"),
                        Targetable = reader.GetBoolean("Targetable"),
                        Price = reader.GetInt32("Price"),
                        Conditions = reader.GetString("Conditions"),
                        Stats = reader.GetString("stats"),
                        UseEffects = reader.GetString("UseEffects"),
                    };

                    lock (ItemsInfos)
                        ItemsInfos.Add(itemInfos);
                }

                reader.Close();
            }

            SilverConsole.WriteLine(String.Format("loaded {0} Items successfully", ItemsInfos.Count), ConsoleColor.Green);
        }

        public static void LoadInventoryItems()
        {
            const string req = "SELECT * FROM inventory_items";

            using (var command = new MySqlCommand(req, GameDbManager.Connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var inventoryItem = new InventoryItem
                    {
                        Id = reader.GetInt16("id"),
                        Character = Characters.Find( x => x.Id == reader.GetInt16("characterId")),
                        ItemInfos = ItemsInfos.Find( x => x.Id == reader.GetInt16("ItemId")),
                        ItemPosition = reader.GetInt16("position"),
                        Quantity = reader.GetInt16("quantity"),
                        Stats = reader.GetString("stats"),
                    };

                    lock (InventoryItems)
                        InventoryItems.Add(inventoryItem);
                }

                reader.Close();
            }

            SilverConsole.WriteLine(String.Format("loaded {0} Characters item successfully", InventoryItems.Count), ConsoleColor.Green);
        }
    }
}