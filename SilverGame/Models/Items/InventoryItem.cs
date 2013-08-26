using System.Collections.Generic;
using System.Linq;
using SilverGame.Database;
using SilverGame.Models.Characters;
using SilverGame.Models.Maps;
using SilverGame.Services;

namespace SilverGame.Models.Items
{
    class InventoryItem
    {
        public int Id { get; set; }
        public Character Character;
        public StatsManager.Position ItemPosition { get; set; }
        public ItemInfos ItemInfos { get; set; }
        public int Quantity { get; set; }
        public List<ItemStats> Stats { get; set; }
        public Map Map { get; set; }
        public int Cell { get; set; }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}",
                Algorithm.DeciToHex(Id),
                Algorithm.DeciToHex(Quantity),
                Algorithm.DeciToHex((int) ItemPosition),
                string.Join(",", Stats));
        }

        public string ItemInfo()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}",
                Algorithm.DeciToHex(Id),
                Algorithm.DeciToHex(ItemInfos.Id),
                Algorithm.DeciToHex(Quantity),
                Algorithm.DeciToHex((int) ItemPosition),
                string.Join(",", Stats));
        }

        public string ToExchangeFormat(int quantity)
        {
            return string.Format("{0}|{1}|{2}|{3}",
                Id,
                quantity,
                ItemInfos.Id,
                string.Join(",", Stats));
        }

        public InventoryItem Copy(StatsManager.Position position = StatsManager.Position.None, int quantity = 1)
        {
            return new InventoryItem
            {
                Id =
                    DatabaseProvider.InventoryItems.Count > 0
                        ? DatabaseProvider.InventoryItems.OrderByDescending(x => x.Id).First().Id + 1
                        : 1,
                Character = this.Character,
                ItemInfos = this.ItemInfos,
                Quantity = quantity,
                Stats = this.Stats,
                ItemPosition = position,
            };
        }

        public bool IsEquiped()
        {
            return (int) ItemPosition > (int) StatsManager.Position.None &&
                   (int) ItemPosition < (int) StatsManager.Position.Bar1;
        }

        public static InventoryItem ExistItem(InventoryItem item, Character character, StatsManager.Position position = StatsManager.Position.None, int quantity = 1)
        {
            return DatabaseProvider.InventoryItems.Find(
                x =>
                    x.ItemInfos == item.ItemInfos &&
                    string.Join(",", x.Stats).Equals(string.Join(",", item.Stats)) &&
                    x.Character == character && x.ItemPosition == position && x.Quantity >= quantity);
        }
    }
}
