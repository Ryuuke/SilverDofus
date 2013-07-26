using System.Collections.Generic;

namespace SilverGame.Models.Items
{
    class ItemInfos
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Weight { get; set; }
        public string Stats { get; set; }
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
    }
}
