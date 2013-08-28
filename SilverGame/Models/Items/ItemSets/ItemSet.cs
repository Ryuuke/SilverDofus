using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Models.Items.Abstract;
using SilverGame.Models.Items.Items;

namespace SilverGame.Models.Items.ItemSets
{
    class ItemSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ItemInfos> Items { get; set; }
        public Dictionary<int, List<Jet>> BonusesDictionary { get; set; }

        public static IEnumerable<KeyValuePair<int, List<Jet>>> ToBonusDictionary(params string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(string.Empty))
                    continue;

                var listItemSetBonus = args[i].Split(';').Select(bonus => new Jet
                {
                    Header = (StatsManager.Effect) int.Parse(bonus.Split(',')[0]),
                    MinValue = int.Parse(bonus.Split(',')[1])
                }).ToList();

                yield return new KeyValuePair<int, List<Jet>>(i + 2, listItemSetBonus);
            }
        }
    }
}
