using SilverGame.Database;
using SilverGame.Models.Items;
using SilverGame.Models.Items.Items;
using SilverGame.Services;

namespace SilverGame.Models.Gifts
{
    class GiftItems
    {
        public int GiftId { get; set; }
        public ItemInfos Item { get; set; }
        public int Quantity { get; set; }

        public string GetGiftFormat()
        {
            return string.Format("{0}~{1}~{2}~{3}",
                Algorithm.DeciToHex(Item.Id),
                Algorithm.DeciToHex(Quantity),
                "1",
                string.Join(",", Item.Stats));
        }
    }
}
