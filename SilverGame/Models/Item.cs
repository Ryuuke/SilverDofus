using System.Collections.Generic;

namespace SilverGame.Models
{
    class Item
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Weight { get; set; }
        public List<ItemStats> Stats { get; set; }
        public string WeaponInfo { get; set; }
        public int Price { get; set; }
        public int Type { get; set; }
        public bool TwoHands { get; set; }
        public string Conditions { get; set; }
        public string UseEffects { get; set; }
        public bool IsEthereal { get; set; }
        public bool Forgemageable { get; set; }
        public bool Targetable { get; set; }
        public bool IsBuff { get; set; }
        public bool Usable { get; set; }

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
