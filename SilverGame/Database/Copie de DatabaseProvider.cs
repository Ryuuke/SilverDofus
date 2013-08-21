//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using SilverGame.Database.Connection;
//using SilverGame.Database.Repository;
//using SilverGame.Models.Accounts;
//using SilverGame.Models.Alignment;
//using SilverGame.Models.Chat;
//using SilverGame.Models.Experience;
//using SilverGame.Models.Gifts;
//using MySql.Data.MySqlClient;
//using SilverGame.Models.Characters;
//using SilverGame.Models.Items;
//using SilverGame.Models.Maps;
//using SilverGame.Services;

//namespace SilverGame.Database
//{
//    internal class DatabaseProvider
//    {
//        public static int ServerId;
//        public static readonly List<Account> Accounts = new List<Account>();
//        public static readonly List<Alignment> Alignments = new List<Alignment>();
//        public static readonly List<StatsManager> StatsManager = new List<StatsManager>();
//        public static readonly List<AccountCharacters> AccountCharacters = new List<AccountCharacters>();
//        public static readonly List<Character> Characters = new List<Character>();
//        public static readonly List<InventoryItem> InventoryItems = new List<InventoryItem>();
//        public static readonly List<ItemInfos> ItemsInfos = new List<ItemInfos>();
//        public static readonly List<Gift> Gifts = new List<Gift>();
//        public static readonly List<KeyValuePair<Account, Gift>> AccountGifts = new List<KeyValuePair<Account, Gift>>();
//        public static readonly List<GiftItems> ItemGift = new List<GiftItems>();
//        public static readonly List<Experience> Experiences = new List<Experience>();
//        public static readonly List<Map> Maps = new List<Map>();
//        public static readonly List<MapTrigger> MapTriggers = new List<MapTrigger>();

//        private static Task _loadingTask;

//        public static void LoadDatabase()
//        {
//            SilverConsole.WriteLine("loading database resources... \n" , ConsoleColor.DarkGreen);

//            AccountRepository.UpdateAccount(false);

//            LoadServerId();
//            LoadAccounts();
//            LoadAlignments();
//            LoadStatsManager();
//            LoadMaps();
//            LoadTriggers();
//            LoadCharacters();
//            LoadCharactersAccount();
//            LoadItemInfos();
//            LoadInventoryItems();
//            LoadGifts();
//            LoadAccountGifts();
//            LoadExperience();

//        }

//        private static void LoadServerId()
//        {
//            lock (RealmDbManager.Lock)
//            {
//                const string query = "SELECT id FROM gameservers WHERE ServerKey=@key LIMIT 1";

//                try
//                {
//                    using (var command = new MySqlCommand(query, RealmDbManager.Connection))
//                    {
//                        command.Parameters.Add(new MySqlParameter("@key", Config.Get("Game_key")));

//                        var reader = command.ExecuteReader();

//                        if (reader.Read())
//                            ServerId = reader.GetInt16("id");

//                        reader.Close();
//                    }
//                }
//                catch (Exception e)
//                {
//                    SilverConsole.WriteLine("SQL Error : "+ e.Message, ConsoleColor.Red);
//                }
//            }
//        }

//        public static void LoadAccounts()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Accounts"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM accounts";

//            using (var command = new MySqlCommand(query, RealmDbManager.Connection))
//            {
//                var reader = command.ExecuteReader();

//                while (reader.Read())
//                {
//                    try
//                    {
//                        var account = new Account
//                        {
//                            Id = reader.GetInt32("id"),
//                            Username = reader.GetString("username"),
//                            Password = reader.GetString("pass"),
//                            Pseudo = reader.GetString("pseudo"),
//                            Question = reader.GetString("question"),
//                            Reponse = reader.GetString("reponse"),
//                            Connected = reader.GetBoolean("connected"),
//                            GmLevel = reader.GetInt16("gmLevel"),
//                            BannedUntil =
//                                Convert.IsDBNull(reader["bannedUntil"])
//                                    ? (DateTime?) null
//                                    : reader.GetDateTime("bannedUntil"),
//                            Subscription =
//                                Convert.IsDBNull(reader["subscription"])
//                                    ? (DateTime?) null
//                                    : reader.GetDateTime("subscription"),
//                        };

//                        lock (Accounts)
//                            Accounts.Add(account);
//                    }
//                    catch (Exception e)
//                    {
//                        SilverConsole.WriteLine("SQL Error " + e.Message, ConsoleColor.Red);
//                        Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//                    }
//                }

//                reader.Close();
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(String.Format(" Loaded {0} Accounts successfully", Accounts.Count),
//                ConsoleColor.Green);
//        }

//        public static void LoadCharactersAccount()
//        {
//            lock (RealmDbManager.Lock)
//            {
//                var tableResult = new List<KeyValuePair<int, string>>();

//                const string query = "SELECT * FROM characters WHERE gameserverId =@ServerId";

//                try
//                {
//                    using (var command = new MySqlCommand(query, RealmDbManager.Connection))
//                    {
//                        command.Parameters.Add(new MySqlParameter("@ServerId", ServerId));

//                        var reader = command.ExecuteReader();

//                        while (reader.Read())
//                        {
//                            tableResult.Add(new KeyValuePair<int, string>(
//                                reader.GetInt16("AccountId"),
//                                reader.GetString("characterName")
//                                ));
//                        }

//                        reader.Close();
//                    }

//                    lock (AccountCharacters)
//                    {
//                        foreach (var value in tableResult)
//                        {
//                            AccountCharacters.Add(new AccountCharacters
//                            {
//                                Account = Accounts.Find(x => x.Id == value.Key),
//                                Character = Characters.Find(x => x.Name.Equals(value.Value))
//                            });
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//                    Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//                }
//            }
//        }

//        public static void LoadCharacters()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Characters"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM Characters";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    lock (Characters)
//                    {
//                        while (reader.Read())
//                        {
//                            Characters.Add(new Character
//                            {
//                                Id = reader.GetInt16("id"),
//                                Name = reader.GetString("name"),
//                                Classe = (Character.Class) reader.GetInt16("classe"),
//                                Color1 = reader.GetInt32("color1"),
//                                Color2 = reader.GetInt32("color2"),
//                                Color3 = reader.GetInt32("color3"),
//                                Level = reader.GetInt16("level"),
//                                Sex = reader.GetInt16("sex"),
//                                Skin = reader.GetInt16("skin"),
//                                Stats = StatsManager.Find(x => x.Id == reader.GetInt16("statsId")),
//                                PdvNow = reader.GetInt16("pdvNow"),
//                                PdvMax = (reader.GetInt16("level") - 1)*Character.GainHpPerLvl + Character.BaseHp,
//                                Map = Maps.Find(x => x.Id == reader.GetInt32("mapId")),
//                                MapCell = reader.GetInt32("cellId"),
//                                Direction = reader.GetInt16("direction"),
//                                Channels = reader.GetString("channels").Split(',').Select(channel => new Channel
//                                {
//                                    Header = char.Parse(channel.Split(';')[0]),
//                                    State = bool.Parse(channel.Split(';')[1]),
//                                }).ToList(),
//                            });
//                        }

//                        reader.Close();
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(String.Format(" Loaded {0} Characters successfully", Characters.Count),
//                ConsoleColor.Green);
//        }

//        public static void LoadItemInfos()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Items"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM items_infos";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    while (reader.Read())
//                    {
//                        var itemInfos = new ItemInfos
//                        {
//                            Id = reader.GetInt16("ID"),
//                            Name = reader.GetString("Name"),
//                            ItemType = (StatsManager.ItemType)reader.GetInt16("Type"),
//                            Level = reader.GetInt16("Level"),
//                            Weight = reader.GetInt16("Weight"),
//                            WeaponInfo = reader.GetString("WeaponInfo"),
//                            TwoHands = reader.GetBoolean("TwoHands"),
//                            IsEthereal = reader.GetBoolean("IsEthereal"),
//                            IsBuff = reader.GetBoolean("IsBuff"),
//                            Usable = reader.GetBoolean("Usable"),
//                            Targetable = reader.GetBoolean("Targetable"),
//                            Price = reader.GetInt32("Price"),
//                            Conditions = reader.GetString("Conditions"),
//                            Stats = ItemStats.ToStats(reader.GetString("stats")),
//                            UseEffects = reader.GetString("UseEffects"),
//                        };

//                        lock (ItemsInfos)
//                            ItemsInfos.Add(itemInfos);
//                    }

//                    reader.Close();
//                }
//            }
//            catch (Exception e )
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }


//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(String.Format(" Loaded {0} Items successfully", ItemsInfos.Count), ConsoleColor.Green);
//        }

//        public static void LoadInventoryItems()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Inventory items"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM inventory_items";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    while (reader.Read())
//                    {
//                        var inventoryItem = new InventoryItem
//                        {
//                            Id = reader.GetInt16("id"),
//                            Character = Characters.Find(x => x.Id == reader.GetInt16("characterId")),
//                            ItemInfos = ItemsInfos.Find(x => x.Id == reader.GetInt16("ItemId")),
//                            ItemPosition = (StatsManager.Position)reader.GetInt16("position"),
//                            Quantity = reader.GetInt16("quantity"),
//                            Stats = ItemStats.ToStats(reader.GetString("stats")),
//                        };

//                        lock (InventoryItems)
//                            InventoryItems.Add(inventoryItem);
//                    }

//                    reader.Close();
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }
//            lock (Characters)
//                foreach (var character in Characters)
//                    character.CalculateItemStats();

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(String.Format(" Loaded {0} Inventory items successfully", InventoryItems.Count),
//                ConsoleColor.Green);
//        }

//        public static void LoadGifts()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Gifts"));
//            _loadingTask.Start();

//            var query = "SELECT * FROM gift_items";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    lock (ItemsInfos)
//                    {
//                        while (reader.Read())
//                        {
//                            ItemGift.Add(new GiftItems
//                            {
//                                GiftId = reader.GetInt16("giftId"),
//                                Item = ItemsInfos.Find(x => x.Id == reader.GetInt16("itemId")),
//                                Quantity = reader.GetInt16("quantity")
//                            });
//                        }

//                        reader.Close();
//                    }
//                }

//                query = "SELECT * FROM gift";

//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    while (reader.Read())
//                    {
//                        lock (Gifts)
//                        {
//                            Gifts.Add(new Gift
//                            {
//                                Id = reader.GetInt16("id"),
//                                Title = reader.GetString("title"),
//                                Description = reader.GetString("Description"),
//                                PictureUrl = reader.GetString("pictureUrl"),
//                                Items = ItemGift.FindAll(x => x.GiftId == reader.GetInt16("id")).Select(x => x.Item)
//                            });
//                        }
//                    }

//                    reader.Close();
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Gifts Sucessfully", Gifts.Count), ConsoleColor.Green);
//        }

//        public static void LoadAccountGifts()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Account Gifts"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM account_gifts";

//            var listResult = new List<KeyValuePair<int, int>>();

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    while (reader.Read())
//                    {
//                        listResult.Add(new KeyValuePair<int, int>(
//                            reader.GetInt16("accountId"),
//                            reader.GetInt16("giftId")
//                            ));
//                    }

//                    reader.Close();
//                }

//                lock (AccountGifts)
//                {
//                    foreach (var keyValuePair in listResult)
//                    {
//                        AccountGifts.Add(new KeyValuePair<Account, Gift>(
//                            Accounts.Find(x => x.Id == keyValuePair.Key),
//                            Gifts.Find(x => x.Id == keyValuePair.Value)
//                            ));
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Accounts Gifts Sucessfully", AccountGifts.Count), ConsoleColor.Green);
//        }

//        private static void LoadExperience()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Levels"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM exp_data";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    lock (Experiences)
//                    {
//                        while (reader.Read())
//                        {
//                            Experiences.Add(new Experience
//                            {
//                                Level = reader.GetInt32("Level"),
//                                CharacterExp = reader.GetInt64("Character"),
//                                JobExp = reader.GetInt32("Job"),
//                                MountExp = reader.GetInt32("Mount"),
//                                PvpExp = reader.GetInt32("Pvp")
//                            });
//                        }

//                        reader.Close();
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : "+ e.Message, ConsoleColor.Red);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Levels Sucessfully", Experiences.Count), ConsoleColor.Green);
//        }

//        private static void LoadAlignments()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Characters Alignment"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM alignments";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    lock (Alignments)
//                    {
//                        while (reader.Read())
//                        {
//                            Alignments.Add(new Alignment
//                            {
//                                Id = reader.GetInt16("Id"),
//                                Type = reader.GetInt16("Type"),
//                                Honor = reader.GetInt16("Honor"),
//                                Deshonor = reader.GetInt16("Deshonor"),
//                                Grade = reader.GetInt16("Grade"),
//                                Level = reader.GetInt16("Level"),
//                                Enabled = reader.GetBoolean("Enabled")
//                            });
//                        }

//                        reader.Close();
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Characters Alignment Sucessfully", Alignments.Count), ConsoleColor.Green);
//        }


//        private static void LoadStatsManager()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Character Stats"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM character_stats";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    lock (StatsManager)
//                    {
//                        while (reader.Read())
//                        {
//                            StatsManager.Add(new StatsManager
//                            {
//                                Id = reader.GetInt16("id"),
//                                Vitality = new GeneralStats { Base = reader.GetInt16("Vitality") },
//                                Wisdom = new GeneralStats { Base = reader.GetInt16("Wisdom") },
//                                Strength = new GeneralStats { Base = reader.GetInt16("Strenght") },
//                                Intelligence = new GeneralStats { Base = reader.GetInt16("Intelligence") },
//                                Chance = new GeneralStats { Base = reader.GetInt16("Chance") },
//                                Agility = new GeneralStats { Base = reader.GetInt16("Agility") },
//                            });
//                        }

//                        reader.Close();
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//                Logs.LogWritter(Constant.ErrorsFolder, "SQL Error :" + e.Message);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Characters Stats Sucessfully", StatsManager.Count), ConsoleColor.Green);
//        }

//        private static void LoadMaps()
//        {
//            _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Maps"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM maps_data";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();
                    

//                    lock (Maps)
//                    {
//                        while (reader.Read())
//                        {
//                            var newMap = new Map
//                            {
//                                Id = reader.GetInt32("id"),
//                                Width = reader.GetInt32("width"),
//                                Height = reader.GetInt16("height"),
//                                PosX = reader.GetInt16("posX"),
//                                PosY = reader.GetInt16("posY"),
//                                MapData = reader.GetString("mapData"),
//                                Key = reader.GetString("decryptkey"),
//                                Time = reader.GetString("createTime"),
//                                Subarea = reader.GetInt16("subarea"),
//                                NeedRegister = reader.GetBoolean("needRegister"),
//                            };

//                            newMap.Cells = newMap.UncompressDatas();

//                            Maps.Add(newMap);
//                        }

//                        reader.Close();
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Maps Sucessfully", Maps.Count), ConsoleColor.Green);
//        }

//        private static void LoadTriggers()
//        {
//           _loadingTask = new Task(() => SilverConsole.StartLoad("Loading Triggers"));
//            _loadingTask.Start();

//            const string query = "SELECT * FROM maps_triggers";

//            try
//            {
//                using (var command = new MySqlCommand(query, GameDbManager.Connection))
//                {
//                    var reader = command.ExecuteReader();

//                    lock (MapTriggers)
//                    {
//                        while (reader.Read())
//                        {
//                            MapTriggers.Add(new MapTrigger
//                            {
//                                Map = Maps.Find(x => x.Id == reader.GetInt32("MapID")),
//                                Cell = reader.GetInt32("CellID"),
//                                NewMap = Maps.Find(x => x.Id == reader.GetInt32("NewMap")),
//                                NewCell = reader.GetInt32("NewCell")
//                            });
//                        }
//                    }

//                    reader.Close();
//                }
//            }
//            catch (Exception e)
//            {
//                SilverConsole.WriteLine("SQL Error : " + e.Message, ConsoleColor.Red);
//            }

//            SilverConsole.StopLoad();

//            SilverConsole.WriteLine(string.Format(" Loaded {0} Triggers Sucessfully", MapTriggers.Count), ConsoleColor.Green);
//        }
//    }
//}