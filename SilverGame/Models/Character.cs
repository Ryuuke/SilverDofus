using System.Collections.Generic;

namespace SilverGame.Models
{
    class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Classe { get; set; }
        public int Level { get; set; }
        public int Skin { get; set; }
        public int Sexe { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public List<Item> ItemList { get; set; }
    }
}