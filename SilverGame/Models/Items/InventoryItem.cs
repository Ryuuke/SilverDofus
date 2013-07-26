using SilverGame.Models.Characters;
using SilverGame.Services;

namespace SilverGame.Models.Items
{
    class InventoryItem
    {
        public int Id { get; set; }
        public Character Character;
        public int ItemPosition { get; set; }
        public ItemInfos ItemInfos { get; set; }
        public int Quantity { get; set; }
        public string Stats { get; set; }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}",
                Algorithm.DeciToHex(Id),
                Algorithm.DeciToHex(Quantity),
                Algorithm.DeciToHex(ItemPosition),
                Stats);
        }

        public enum Position
        {
            None = -1,
            Amulet = 0,
            Weapon = 1,
            Ring1 = 2,
            Belt = 3,
            Ring2 = 4,
            Boot = 5,
            Hat = 6,
            Cloak = 7,
            Pet = 8,
            Dofus1 = 9,
            Dofus2 = 10,
            Dofus3 = 11,
            Dofus4 = 12,
            Dofus5 = 13,
            Dofus6 = 14,
            Shield = 15,
            Mount = 16,

            UsableObject1 = 23,
            UsableObject2 = 24,
            UsableObject3 = 25,
            UsableObject4 = 26,
            UsableObject5 = 27,
            UsableObject6 = 28,
            UsableObject7 = 29,
            UsableObject8 = 30,
            UsableObject9 = 31,
            UsableObject10 = 32,
            UsableObject11 = 33,
            UsableObject12 = 34,
            UsableObject13 = 35,
            UsableObject14 = 36,
        }
    }
}
