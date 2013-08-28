using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Database;
using SilverGame.Database.Repository;
using SilverGame.Models.Characters;
using SilverGame.Models.Items.ItemSets;

namespace SilverGame.Models.Items.Items
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
        public StatsManager.ItemType ItemType { get; set; }
        public bool TwoHands { get; set; }
        public string Conditions { get; set; }
        public string UseEffects { get; set; }
        public bool IsEthereal { get; set; }
        public bool Forgemageable { get; set; }
        public bool Targetable { get; set; }
        public bool IsBuff { get; set; }
        public bool Usable { get; set; }


        public void Generate(Character character, int quantity = 1)
        {
            var item = new InventoryItem
            {
                Id = DatabaseProvider.InventoryItems.Count > 0
                    ? DatabaseProvider.InventoryItems.OrderByDescending(x => x.Id).First().Id + 1
                    : 1,
                Character = character,
                ItemInfos = this,
                ItemPosition = StatsManager.Position.None,
                Stats = ItemStats.GenerateRandomStats(Stats).ToList(),
                Quantity = quantity
            };

            var existItem = InventoryItem.ExistItem(item, item.Character, item.ItemPosition);

            if (existItem != null)
            {
                existItem.Quantity += 1;
                InventoryItemRepository.Update(existItem);
            }
            else
                InventoryItemRepository.Create(item, true);
        }

        public bool HasSet()
        {
            return DatabaseProvider.ItemSets.Any(x => x.Items.Any(y => y.Id == Id));
        }

        public ItemSet GetSet()
        {
            return !HasSet() ? null : DatabaseProvider.ItemSets.Find(x => x.Items.Any(y => y.Id == Id));
        }
    }
}
