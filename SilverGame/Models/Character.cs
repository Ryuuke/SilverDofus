using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Character()
        {
            Level = int.Parse(Services.Config.Get("Starting_level"));
        }
    }
}
