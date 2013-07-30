using System.Collections.Generic;
using SilverGame.Models.Characters;
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
    }
}
