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

        public string InfosWheneChooseCharacter()
        {
            return string.Format("|{0};{1};{2};{3};{4};{5};{6};{7};0;{8};;;",
                Id, Name, Level, Skin, Color1, Color2, Color3,
                GetItemsWheneChooseCharacter(), DatabaseProvider.ServerId);
        }
            
        private string GetItemsWheneChooseCharacter()
        {
            var items = string.Empty;

            var inventoryItems = DatabaseProvider.InventoryItems.FindAll(x => x.Character.Id == Id);

            if (inventoryItems.Any(x => x.ItemPosition == ItemManager.Position.Arme))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == ItemManager.Position.Arme).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == ItemManager.Position.Coiffe))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == ItemManager.Position.Coiffe).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == ItemManager.Position.Cape))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == ItemManager.Position.Cape).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == ItemManager.Position.Familier))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == ItemManager.Position.Familier).ItemInfos.Id);

            items += ",";

            if (inventoryItems.Any(x => x.ItemPosition == ItemManager.Position.Bouclier))
                items += Algorithm.DeciToHex(inventoryItems.Find(x => x.ItemPosition == ItemManager.Position.Bouclier).ItemInfos.Id);

            return items;
        }
    }
}