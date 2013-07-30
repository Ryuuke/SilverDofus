using System.Linq;
using SilverGame.Database;
using SilverGame.Models.Items;
using SilverGame.Services;

namespace SilverGame.Models.Characters
{
    class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Classe { get; set; }
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
        public int Vitality { get; set; }
        public int Wisdom { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Chance { get; set; }
        public int Agility { get; set; }
        public StatsManager Stats { get; set; }

        public void CalculateItemStats()
        {
            Stats.Calculate(this);
        }

        public string InfosWheneChooseCharacter()
        {
            return string.Format("|{0};{1};{2};{3};{4};{5};{6};{7};0;{8};;;",
                Id, Name, Level, Skin, Color1, Color2, Color3,
                GetItemsWheneChooseCharacter(), DatabaseProvider.ServerId);
        }

        public string InfosWheneSelectedCharacter()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9};",
                Id, Name, Level, Classe, Sex, Skin, Color1, Color2, Color3, GetItemsWheneSelectedCharacter());
        }

        private string GetItemsWheneSelectedCharacter()
        {
            var items = DatabaseProvider.InventoryItems.FindAll(x => x.Character.Id == x.Id);

            return items.Aggregate(string.Empty, (current, inventoryItem) => current + inventoryItem.ItemInfo());
        }

        private string GetItemsWheneChooseCharacter()
        {
            var items = string.Empty;

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
            var stats = string.Empty;

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
            return string.Format("{0},{1},{2}",
                    Exp, 
                    DatabaseProvider.Experiences.Find(x => x.Level == Level).CharacterExp,
                    DatabaseProvider.Experiences.Find(x => x.Level == Level+1).CharacterExp
                );
        }

        private int GetInitiative()
        {
            return (PdvMax/4 + Stats.Initiative.Items)*(PdvNow / PdvMax);
        }


        private int GetProspection()
        {
            return Chance/10 + Classe == 3 ? 120 : 100 + Stats.Prospection.Items;
        }

        private string GetPa()
        {
            return string.Format("{0},{1},0,0,{2}",
                Level < 100 ? 6 : 7, Stats.Pa.Items, Level < 100 ? 6 + Stats.Pa.Items : 7 + Stats.Pa.Items);
        }


        private string GetPm()
        {
            return string.Format("{0},{1},0,0,{2}",
                3, Stats.Pm.Items, 3 + Stats.Pm.Items);
        }

        public enum ClassHp
        {
            Hp = 50,
        }
    }
}