using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Database;
using SilverGame.Database.Repository;
using SilverGame.Models.Characters;
using SilverGame.Services;

namespace SilverGame.Models.Items
{
    class ItemInfos
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Weight { get; set; }
        public List<ItemStats> Stats { get; set; }
        public string WeaponInfo { get; set; }
        public int Price { get; set; }
        public ItemManager.ItemType ItemType { get; set; }
        public bool TwoHands { get; set; }
        public string Conditions { get; set; }
        public string UseEffects { get; set; }
        public bool IsEthereal { get; set; }
        public bool Forgemageable { get; set; }
        public bool Targetable { get; set; }
        public bool IsBuff { get; set; }
        public bool Usable { get; set; }

        public string GetGiftFormat(int id)
        {
            return string.Format("{0}~{1}~{2}~{3}",
                Algorithm.DeciToHex(Id),
                Algorithm.DeciToHex(DatabaseProvider.ItemGift.Find(x => x.GiftId == id && x.Item.Id == Id).Quantity),
                "1",
                string.Join(",", Stats));
        }

        public string Generate(Character character, int quantity)
        {
            var item = new InventoryItem
            {
                Id = DatabaseProvider.InventoryItems.Count > 0
                    ? DatabaseProvider.InventoryItems.OrderByDescending(x => x.Id).First().Id + 1
                    : 1,
                Character = character,
                ItemInfos = this,
                ItemPosition = ItemManager.Position.None,
                Stats = ItemStats.GenerateRandomStats(this.Stats),
                Quantity = 1
            };

            var existItem =
                DatabaseProvider.InventoryItems.Find(
                    x => x.ItemInfos.Id == item.Id && x.Stats.Equals(item.Stats) && x.Character.Id == item.Character.Id && x.ItemPosition == item.ItemPosition);

            if (existItem != null)
            {
                existItem.Quantity += 1;
                InventoryItemRepository.Update(existItem);
            }
            else
                InventoryItemRepository.Create(item);
            
            
            return item.ItemInfo();
        }
    }
}
