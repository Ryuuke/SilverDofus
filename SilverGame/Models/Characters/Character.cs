using System.Linq;
using SilverGame.Database;
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

        public string InfosWheneChooseCharacter()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};0;{8};1;1;",
                Id, Name, Level, Skin, Color1, Color2, Color3,
                GetItemsWheneChooseCharacter(), DatabaseProvider.ServerId);
        }

        private string GetItemsWheneChooseCharacter()
        {
            var items = string.Empty;

            var inventoryItems = DatabaseProvider.InventoryItems.FindAll(x => x.Character.Id == Id);

            if (inventoryItems.Any(x => x.ItemPosition == 1))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == 1).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == 6))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == 6).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == 7))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == 7).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == 8))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == 8).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == 15))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == 15).ItemInfos.Id);

            return items;
        }
    }
}