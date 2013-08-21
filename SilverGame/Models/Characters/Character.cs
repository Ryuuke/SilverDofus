using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverGame.Database;
using SilverGame.Database.Repository;
using SilverGame.Models.Chat;
using SilverGame.Models.Items;
using SilverGame.Models.Maps;
using SilverGame.Network.Game;
using SilverGame.Services;

namespace SilverGame.Models.Characters
{
    class Character
    {
        public const int BaseHp = 50;
        public const int GainHpPerLvl = 5;

        public int Id { get; set; }
        public string Name { get; set; }
        public Class Classe { get; set; }
        public int Level { get; set; }
        public int Skin { get; set; }
        public int Sex { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public int Kamas { get; set; }
        public int StatsPoints { get; set; }
        public int SpellPoints { get; set; }
        public long Exp { get; set; }
        public Alignment.Alignment Alignment { get; set; }
        public int PdvMax { get; set; }
        public int PdvNow { get; set; }
        public int Energy { get; set; }
        public int EnergyMax { get; set; }
        public StatsManager Stats { get; set; }
        public Map Map { get; set; }
        public int MapCell { get; set; }
        public int Direction { get; set; }
        public int CellDestination { get; set; }
        public List<Channel> Channels { get; set; }

        public void CalculateItemStats()
        {
            // TODO : Calculate Base Stats
            Stats.Calculate(this);
        }

        public string InfosWheneChooseCharacter()
        {
            return String.Format("|{0};{1};{2};{3};{4};{5};{6};{7};0;{8};;;",
                Id, Name, Level, Skin, Algorithm.DeciToHex(Color1), Algorithm.DeciToHex(Color2), Algorithm.DeciToHex(Color3),
                GetItemsWheneChooseCharacter(), DatabaseProvider.ServerId);
        }

        public string InfosWheneSelectedCharacter()
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9};",
                Id, Name, Level, Classe, Sex, Skin, Algorithm.DeciToHex(Color1), Algorithm.DeciToHex(Color2), Algorithm.DeciToHex(Color3), GetItemsWheneSelectedCharacter());
        }

        private string GetItemsWheneSelectedCharacter()
        {
            var items = DatabaseProvider.InventoryItems.FindAll(x => x.Character.Id == Id);

            return items.Aggregate(String.Empty, (current, inventoryItem) => string.Format("{0}{1};", current, inventoryItem.ItemInfo()));
        }

        public string GetItemsWheneChooseCharacter()
        {
            var items = String.Empty;

            var inventoryItems = DatabaseProvider.InventoryItems.FindAll(x => x.Character.Id == Id);

            if (inventoryItems.Count == 0)
                return ",,,,";

            if (inventoryItems.Any(x => x.ItemPosition == StatsManager.Position.Arme))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == StatsManager.Position.Arme).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == StatsManager.Position.Coiffe))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == StatsManager.Position.Coiffe).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == StatsManager.Position.Cape))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == StatsManager.Position.Cape).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == StatsManager.Position.Familier))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == StatsManager.Position.Familier).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == StatsManager.Position.Bouclier))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == StatsManager.Position.Bouclier).ItemInfos.Id);
           
            return items;
        }

        public string GetStats()
        {
            var stats = String.Empty;

            stats += GetExperiance();

            stats += "|" + Kamas;

            stats += "|" + StatsPoints;

            stats += "|" + SpellPoints;

            stats += "|" + Alignment;

            stats += "|" + PdvNow + "," + PdvMax;

            stats += "|" + Energy + "," + EnergyMax;

            stats += "|" + GetInitiative() + "|" + GetProspection();

            stats += "|" + GetPa();

            stats += "|" + GetPm();

            stats += "|" + Stats.Strength;

            stats += "|" + Stats.Vitality;

            stats += "|" + Stats.Wisdom;

            stats += "|" + Stats.Chance;

            stats += "|" + Stats.Agility;

            stats += "|" + Stats.Intelligence;

            stats += "|" + Stats.Po;

            stats += "|" + Stats.MaxInvoc;

            stats += "|" + Stats.Damage;

            stats += "|" + Stats.PhysicalDamage;

            stats += "|" + Stats.MagicDamage;

            stats += "|" + Stats.PercentDamage;

            stats += "|" + Stats.Heal;

            stats += "|" + Stats.TrapDamage;

            stats += "|" + Stats.TrapPercentDamage;

            stats += "|" + Stats.ReturnDamage;

            stats += "|" + Stats.CriticalDamage;

            stats += "|" + Stats.FailDamage;

            stats += "|" + Stats.DodgePa;

            stats += "|" + Stats.DodgePm;

            stats += "|" + Stats.ReduceDamageNeutral;

            stats += "|" + Stats.ReduceDamagePercentNeutral;

            stats += "|" + Stats.ReduceDamagePvpNeutral;

            stats += "|" + Stats.ReduceDamagePercentPvPNeutral;

            stats += "|" + Stats.ReduceDamageStrength;

            stats += "|" + Stats.ReduceDamagePercentStrenght;

            stats += "|" + Stats.ReduceDamagePvpStrength;

            stats += "|" + Stats.ReduceDamagePercentPvPStrenght;

            stats += "|" + Stats.ReduceDamageChance;

            stats += "|" + Stats.ReduceDamagePercentChance;

            stats += "|" + Stats.ReduceDamagePvpChance;

            stats += "|" + Stats.ReduceDamagePercentPvPChance;

            stats += "|" + Stats.ReducDamageAir;

            stats += "|" + Stats.ReduceDamagePercentAir;

            stats += "|" + Stats.ReducDamagePvpAir;

            stats += "|" + Stats.ReduceDamagePercentPvPAir;

            stats += "|" + Stats.ReduceDamageIntelligence;

            stats += "|" + Stats.ReduceDamagePercentIntelligence;

            stats += "|" + Stats.ReduceDamagePvpIntelligence;

            stats += "|" + Stats.ReduceDamagePercentPvPIntelligence;

            return stats;
        }

        private string GetExperiance()
        {
            return String.Format("{0},{1},{2}",
                    Exp, 
                    DatabaseProvider.Experiences.Find(x => x.Level == Level).CharacterExp,
                    DatabaseProvider.Experiences.Find(x => x.Level == Level+1).CharacterExp);
        }

        private int GetInitiative()
        {
            var initiative = 0;

            initiative += Level + Stats.Initiative.GetTotal();

            initiative += Stats.Intelligence.GetTotal() > 0 ? (int) Math.Floor(1.5*Stats.Intelligence.GetTotal()) : 0;
            initiative += Stats.Intelligence.GetTotal() > 0 ? (int) Math.Floor(1.5 * Stats.Agility.GetTotal()) : 0;
            initiative += Stats.Intelligence.GetTotal() > 0 ? Stats.Wisdom.GetTotal() : 0;
            initiative += Stats.Intelligence.GetTotal() > 0 ? Stats.Strength.GetTotal() : 0;
            initiative += Stats.Intelligence.GetTotal() > 0 ? Stats.Chance.GetTotal() : 0;

            return initiative;
        }


        private int GetProspection()
        {
            return Stats.Chance.GetTotal()/10 + Classe == Class.Enutrof ? Constant.BaseProspectionEnu : Constant.BaseProspection + Stats.Prospection.GetTotal();
        }

        private string GetPa()
        {
            return String.Format("{0},{1},0,0,{2}",
                Level < Constant.HighLevel ? Constant.BasePa : Constant.BaseHighLevelPa, Stats.Pa.Items, Level < Constant.HighLevel ? Constant.BasePa + Stats.Pa.Items : Constant.BaseHighLevelPa + Stats.Pa.Items);
        }

        private string GetPm()
        {
            return String.Format("{0},{1},0,0,{2}",
                Constant.BasePm, Stats.Pm.Items, Constant.BasePm + Stats.Pm.Items);
        }

        public int GetCurrentWeight()
        {

            return DatabaseProvider.InventoryItems.FindAll(x => x.Character == this).Sum(inventoryItem => inventoryItem.ItemInfos.Weight*inventoryItem.Quantity);

        }

        public int GetMaxWeight()
        {
            // TODO : Add Job Bonus
            return  Constant.BaseWeight + Stats.Weight.Items + Stats.Strength.GetTotal()*5;
        }

        public void GenerateInfos(int gmLevel)
        {
            // Map
            switch (Classe)
            {
                case Class.Feca:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Feca")));
                    MapCell = int.Parse(Config.Get("StartCell_Feca"));
                    Direction = int.Parse(Config.Get("StartDir_Feca"));
                    break;
                case Class.Osamodas:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Osa")));
                    MapCell = int.Parse(Config.Get("StartCell_Osa"));
                    Direction = int.Parse(Config.Get("StartDir_Osa"));
                    break;
                case Class.Enutrof:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Enu")));
                    MapCell = int.Parse(Config.Get("StartCell_Enu"));
                    Direction = int.Parse(Config.Get("StartDir_Enu"));
                    break;
                case Class.Sram:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Sram")));
                    MapCell = int.Parse(Config.Get("StartCell_Sram"));
                    Direction = int.Parse(Config.Get("StartDir_Sram"));
                    break;
                case Class.Xelor:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Xel")));
                    MapCell = int.Parse(Config.Get("StartCell_Xel"));
                    Direction = int.Parse(Config.Get("StartDir_Xel"));
                    break;
                case Class.Ecaflip:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Eca")));
                    MapCell = int.Parse(Config.Get("StartCell_Eca"));
                    Direction = int.Parse(Config.Get("StartDir_Eca"));
                    break;
                case Class.Eniripsa:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Eni")));
                    MapCell = int.Parse(Config.Get("StartCell_Eni"));
                    Direction = int.Parse(Config.Get("StartDir_Eni"));
                    break;
                case Class.Iop:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Iop")));
                    MapCell = int.Parse(Config.Get("StartCell_Iop"));
                    Direction = int.Parse(Config.Get("StartDir_Iop"));
                    break;
                case Class.Cra:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Cra")));
                    MapCell = int.Parse(Config.Get("StartCell_Cra"));
                    Direction = int.Parse(Config.Get("StartDir_Cra"));
                    break;
                case Class.Sadida:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Sadi")));
                    MapCell = int.Parse(Config.Get("StartCell_Sadi"));
                    Direction = int.Parse(Config.Get("StartDir_Sadi"));
                    break;
                case Class.Sacrieur:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Sacri")));
                    MapCell = int.Parse(Config.Get("StartCell_Sacri"));
                    Direction = int.Parse(Config.Get("StartDir_Sacri"));
                    break;
                case Class.Pandawa:
                    Map = DatabaseProvider.Maps.Find(x => x.Id == int.Parse(Config.Get("StartMap_Panda")));
                    MapCell = int.Parse(Config.Get("StartCell_Panda"));
                    Direction = int.Parse(Config.Get("StartDir_Panda"));
                    break;
            }

            // Create alignment row in database & list
            var alignmentId = DatabaseProvider.StatsManager.Count > 0
                ? DatabaseProvider.StatsManager.OrderByDescending(x => x.Id).First().Id + 1
                : 1;

            Alignment = new Alignment.Alignment {Id = alignmentId};
            AlignmentRepository.Create(Alignment);

            // Create stats row in database & list
            var statsId = DatabaseProvider.StatsManager.Count > 0
                ? DatabaseProvider.StatsManager.OrderByDescending(x => x.Id).First().Id + 1
                : 1;

            Stats = new StatsManager {Id = statsId};
            CharacterStatsRepository.Create(Stats);

            Channels = (gmLevel > 0)
                ? Channel.Headers.Select(channel => new Channel
                {
                    Header = (Channel.ChannelHeader) channel
                }).ToList()
                : Channel.Headers.Where(x => x != (char) Channel.ChannelHeader.AdminChannel)
                    .Select(channel => new Channel
                    {
                        Header = (Channel.ChannelHeader) channel
                    }).ToList();
        }

        public string DisplayChar()
        {
            var builder = new StringBuilder();
            {
                builder.Append(MapCell).Append(";");
                builder.Append(Direction).Append(";0;");
                builder.Append(Id).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Classe).Append(";");
                builder.Append(Skin).Append("^").Append("100").Append(";");

                // TODO : debug align info
                builder.Append(Sex).Append(";").Append("0,0,0").Append(",").Append(Level+Id).Append(";");

                builder.Append(Algorithm.DeciToHex(Color1)).Append(";");
                builder.Append(Algorithm.DeciToHex(Color2)).Append(";");
                builder.Append(Algorithm.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsWheneChooseCharacter()).Append(";"); // Items
                builder.Append("0;"); //Aura
                builder.Append(";;");
                builder.Append(";"); // Guild
                builder.Append(";0;");
                builder.Append(";"); // Mount
            }

            return builder.ToString();
        }

        public void SendToAll(string message)
        {
            lock (GameServer.Lock)
            {
                foreach (var client in GameServer.Clients)
                {
                    client.SendPackets(message);
                }
            }
        }

        public void Disconnect()
        {
            if(this.Map != null)
                this.Map.RemoveCharacter(this);

            CharacterRepository.Update(this);
        }

        public enum Class
        {
            Feca = 1,
            Osamodas = 2,
            Enutrof = 3,
            Sram = 4,
            Xelor = 5,
            Ecaflip =  6,
            Eniripsa = 7,
            Iop = 8,
            Cra = 9,
            Sadida = 10,
            Sacrieur = 11,
            Pandawa = 12,
        }
    }
}