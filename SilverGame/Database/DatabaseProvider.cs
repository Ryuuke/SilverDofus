using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using SilverGame.Database.Connection;
using SilverGame.Database.Repository;
using SilverGame.Models.Accounts;
using SilverGame.Models.Alignment;
using SilverGame.Models.Chat;
using SilverGame.Models.Experience;
using SilverGame.Models.Characters;
using SilverGame.Models.Items.Items;
using SilverGame.Models.Items.ItemSets;
using SilverGame.Models.Maps;
using SilverGame.Models.Subareas;
using SilverGame.Services;

namespace SilverGame.Database
{
    internal static class DatabaseProvider
    {
        public static int ServerId;
        public static readonly List<Account> Accounts = new List<Account>();
        public static readonly List<Alignment> Alignments = new List<Alignment>();
        public static readonly List<StatsManager> StatsManager = new List<StatsManager>();
        public static readonly List<AccountCharacters> AccountCharacters = new List<AccountCharacters>();
        public static readonly List<Character> Characters = new List<Character>();
        public static readonly List<InventoryItem> InventoryItems = new List<InventoryItem>();
        public static readonly List<ItemInfos> ItemsInfos = new List<ItemInfos>();
        public static readonly List<ItemSet> ItemSets = new List<ItemSet>();
        public static readonly List<Experience> Experiences = new List<Experience>();
        public static readonly List<Map> Maps = new List<Map>();
        public static readonly List<MapTrigger> MapTriggers = new List<MapTrigger>();
        public static readonly List<Subarea> Subareas = new List<Subarea>();

        public static void LoadDatabase()
        {
            SilverConsole.WriteLine("Loading Database Resources... \n", ConsoleColor.DarkGreen);

            LoadServerId();

            AccountRepository.UpdateAccount(connected: false);

            var sw = new Stopwatch();

            sw.Start();

                LoadAccounts();
                LoadAlignments();
                LoadStatsManager();
                LoadMaps();
                LoadTriggers();
                LoadSubareas();
                LoadCharacters();
                LoadCharactersAccount();
                LoadItemInfos();
                LoadItemSets();
                LoadInventoryItems();
                LoadExperience();

            sw.Stop();

            SilverConsole.WriteLine(string.Format("Loaded Database Resources Successfully in {0} Sec \n", sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture).Substring(0,4)), ConsoleColor.DarkGreen);
        }

        private static void LoadServerId()
        {
            var task = ExecuteQueryLight(RealmDbManager.GetDatabaseConnection(),
                "SELECT id FROM gameservers WHERE ServerKey=@key LIMIT 1", "Game Server",
                (reader) =>
                {
                    if (reader.Read())
                        ServerId = reader.GetInt16("id");

                    return 1;

                }, (command) => command.Parameters.Add(new MySqlParameter("@key", Config.Get("Game_key"))));

            task.Wait();
        }

        private static void LoadAccounts()
        {
            var task = ExecuteQueryLight(RealmDbManager.GetDatabaseConnection(), "SELECT * FROM accounts", "Accounts",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        Accounts.Add(new Account
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Password = reader.GetString("pass"),
                            Pseudo = reader.GetString("pseudo"),
                            Question = reader.GetString("question"),
                            Reponse = reader.GetString("reponse"),
                            GmLevel = reader.GetInt16("gmLevel"),
                            BannedUntil =
                                Convert.IsDBNull(reader["bannedUntil"])
                                    ? (DateTime?) null
                                    : reader.GetDateTime("bannedUntil"),
                            Subscription =
                                Convert.IsDBNull(reader["subscription"])
                                    ? (DateTime?) null
                                    : reader.GetDateTime("subscription"),
                        });
                    }

                    return Accounts.Count;
                });

            task.Wait();
        }

        private static void LoadCharactersAccount()
        {
            var task = ExecuteQueryLight(RealmDbManager.GetDatabaseConnection(),
                "SELECT * FROM characters WHERE gameserverId =@ServerId", "Characters <<>> Account", (reader) =>
                {
                    var tableResults = new List<KeyValuePair<int, int>>();

                    while (reader.Read())
                    {
                        tableResults.Add(new KeyValuePair<int, int>(
                            reader.GetInt16("AccountId"),
                            reader.GetInt16("characterId")
                            ));
                    }

                    foreach (var tableResult in tableResults)
                    {
                        AccountCharacters.Add(new AccountCharacters
                        {
                            Account = Accounts.Find(x => x.Id == tableResult.Key),
                            Character = Characters.Find(x => x.Id == tableResult.Value)
                        });
                    }

                    return AccountCharacters.Count;

                }, (command) => command.Parameters.Add(new MySqlParameter("@ServerId", ServerId)));

            task.Wait();
        }

        private static void LoadCharacters()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM characters", "Characters",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        Characters.Add(new Character
                        {
                            Id = reader.GetInt16("id"),
                            Name = reader.GetString("name"),
                            Classe = (Character.Class) reader.GetInt16("classe"),
                            Color1 = reader.GetInt32("color1"),
                            Color2 = reader.GetInt32("color2"),
                            Color3 = reader.GetInt32("color3"),
                            Level = reader.GetInt16("level"),
                            Sex = reader.GetInt16("sex"),
                            Skin = reader.GetInt16("skin"),
                            Stats = StatsManager.Find(x => x.Id == reader.GetInt16("statsId")),
                            Alignment = Alignments.Find(x => x.Id == reader.GetInt16("alignmentId")),
                            PdvNow = reader.GetInt16("pdvNow"),
                            PdvMax = reader.GetInt16("pdvNow"),
                            Map = Maps.Find(x => x.Id == reader.GetInt32("mapId")),
                            MapCell = reader.GetInt32("cellId"),
                            Direction = reader.GetInt16("direction"),
                            StatsPoints = reader.GetInt32("statsPoints"),
                            SpellPoints = reader.GetInt32("spellsPoints"),
                            Kamas = reader.GetInt32("kamas"),
                            Channels = reader.GetString("channels").Select(channel => new Channel
                            {
                                Header = (Channel.ChannelHeader) channel
                            }).ToList(),
                        });
                    }

                    return Characters.Count;
                });

            task.Wait();
        }

        private static void LoadItemInfos()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM items_infos", "Items",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        ItemsInfos.Add(new ItemInfos
                        {
                            Id = reader.GetInt16("ID"),
                            Name = reader.GetString("Name"),
                            ItemType = (StatsManager.ItemType) reader.GetInt16("Type"),
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
                            Stats = ItemStats.ToStats(reader.GetString("stats")),
                            UseEffects = reader.GetString("UseEffects"),
                        });
                    }

                    return ItemsInfos.Count;
                });

            task.Wait();
        }


        private static void LoadItemSets()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM items_set",
                "Items Set",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        ItemSets.Add(new ItemSet
                        {
                            Id = reader.GetInt16("id"),
                            Name = reader.GetString("name"),
                            Items =
                                reader.GetString("items")
                                    .Split(',')
                                    .Select(itemId => ItemsInfos.Find(x => x.Id == int.Parse(itemId)))
                                    .ToList(),
                            BonusesDictionary =
                                ItemSet.ToBonusDictionary(reader.GetString("effects2"), reader.GetString("effects3"),
                                    reader.GetString("effects4"), reader.GetString("effects5"),
                                    reader.GetString("effects6"), reader.GetString("effects7"),
                                    reader.GetString("effects8")).ToDictionary(x => x.Key, x => x.Value)
                        });
                    }

                    return ItemSets.Count;
                });

            task.Wait();
        }

        private static void LoadInventoryItems()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM inventory_items",
                "Inventory Items",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        InventoryItems.Add(new InventoryItem
                        {
                            Id = reader.GetInt16("id"),
                            Character = Characters.Find(x => x.Id == reader.GetInt16("characterId")),
                            ItemInfos = ItemsInfos.Find(x => x.Id == reader.GetInt16("ItemId")),
                            ItemPosition = (StatsManager.Position) reader.GetInt16("position"),
                            Quantity = reader.GetInt16("quantity"),
                            Stats = ItemStats.ToStats(reader.GetString("stats")),
                        });
                    }

                    foreach (var character in Characters)
                        character.CalculateItemStats();

                    return InventoryItems.Count;
                });

            task.Wait();
        }

        private static void LoadExperience()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM exp_data", "Levels",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        Experiences.Add(new Experience
                        {
                            Level = reader.GetInt32("Level"),
                            CharacterExp = reader.GetInt64("Character"),
                            JobExp = reader.GetInt32("Job"),
                            MountExp = reader.GetInt32("Mount"),
                            PvpExp = reader.GetInt32("Pvp")
                        });
                    }

                    return Experiences.Count;
                });

            task.Wait();
        }

        private static void LoadAlignments()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM alignments", "Alignments",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        Alignments.Add(new Alignment
                        {
                            Id = reader.GetInt16("Id"),
                            Type = reader.GetInt16("Type"),
                            Honor = reader.GetInt16("Honor"),
                            Deshonor = reader.GetInt16("Deshonor"),
                            Grade = reader.GetInt16("Grade"),
                            Level = reader.GetInt16("Level"),
                            Enabled = reader.GetBoolean("Enabled")
                        });
                    }

                    return Alignments.Count;
                });

            task.Wait();
        }

        private static void LoadStatsManager()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM character_stats",
                "Character Stats",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        StatsManager.Add(new StatsManager
                        {
                            Id = reader.GetInt16("id"),
                            Vitality = new GeneralStats {Base = reader.GetInt16("Vitality")},
                            Wisdom = new GeneralStats {Base = reader.GetInt16("Wisdom")},
                            Strength = new GeneralStats {Base = reader.GetInt16("Strenght")},
                            Intelligence = new GeneralStats {Base = reader.GetInt16("Intelligence")},
                            Chance = new GeneralStats {Base = reader.GetInt16("Chance")},
                            Agility = new GeneralStats {Base = reader.GetInt16("Agility")},
                        });
                    }

                    return StatsManager.Count;
                });

            task.Wait();
        }

        private static void LoadMaps()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM maps_data", "Maps",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        var newMap = new Map
                        {
                            Id = reader.GetInt32("id"),
                            Width = reader.GetInt32("width"),
                            Height = reader.GetInt16("height"),
                            PosX = reader.GetInt16("posX"),
                            PosY = reader.GetInt16("posY"),
                            MapData = reader.GetString("mapData"),
                            Key = reader.GetString("decryptkey"),
                            Time = reader.GetString("createTime"),
                            Subarea = reader.GetInt16("subarea"),
                        };

                        newMap.Cells = newMap.UncompressDatas();

                        Maps.Add(newMap);
                    }

                    return Maps.Count;
                });

            task.Wait();
        }

        private static void LoadTriggers()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM maps_triggers", "Trigger",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        MapTriggers.Add(new MapTrigger
                        {
                            Map = reader.GetInt32("MapID"),
                            Cell = reader.GetInt32("CellID"),
                            NewMap = reader.GetInt32("NewMap"),
                            NewCell = reader.GetInt32("NewCell")
                        });
                    }

                    return MapTriggers.Count;
                });

            task.Wait();
        }

        private static void LoadSubareas()
        {
            var task = ExecuteQueryLight(GameDbManager.GetDatabaseConnection(), "SELECT * FROM subareas", "Subareas",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        Subareas.Add(new Subarea
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            NeedRegistered = reader.GetBoolean("needRegistered")
                        });
                    }

                    return Subareas.Count;
                });

            task.Wait();
        }

        private static async Task ExecuteQueryLight(MySqlConnection connection, string query,
            string loadingItemName,
            Func<MySqlDataReader, int> operation, Action<MySqlCommand> paramAction = null)
        {
            SilverConsole.StartLoad("Loading " + loadingItemName);

            var countListValue = 0;

            try
            {
                using (connection)
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (paramAction != null)
                            paramAction(command);

                        var reader = command.ExecuteReader();

                        var task = new Task<int>(() => operation(reader));

                        task.Start();

                        countListValue = await task;

                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
                Logs.LogWritter(Constant.ErrorsFolder, string.Format("SQL Error : {0}", e.Message));
            }

            SilverConsole.StopLoad();

            SilverConsole.WriteLine(string.Format(" Loaded {0} {1} Sucessfully", countListValue, loadingItemName),
                ConsoleColor.Green);
        }
    }
}