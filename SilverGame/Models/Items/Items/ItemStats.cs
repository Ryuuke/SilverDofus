using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Services;

namespace SilverGame.Models.Items.Items
{
    class ItemStats : Abstract.Jet
    {
        public int MaxValue { get; set; }
        public string JetDecimal = "0d0+0";

        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}",
                Algorithm.DeciToHex((int) Header),
                Algorithm.DeciToHex(MinValue),
                Algorithm.DeciToHex(MaxValue),
                "0",
                JetDecimal);
        }

        public static IEnumerable<ItemStats> GenerateRandomStats(IEnumerable<ItemStats> stats)
        {
            var rand = new Random();

            foreach (var itemStats in stats)
            {
                if (StatsManager.WeaponEffect.Contains(itemStats.Header))
                {
                    yield return new ItemStats
                    {
                        Header = itemStats.Header,
                        MinValue = itemStats.MinValue,
                        MaxValue = itemStats.MaxValue,
                        JetDecimal = itemStats.JetDecimal
                    };
                }
                else
                {
                    yield return new ItemStats
                    {
                        Header = itemStats.Header,
                        MinValue =
                            itemStats.MaxValue == 0
                                ? itemStats.MinValue
                                : rand.Next(itemStats.MinValue, itemStats.MaxValue),
                        MaxValue = 0,
                        JetDecimal = itemStats.JetDecimal,
                    };
                }
            }
        }

        public static List<ItemStats> ToStats(string stats)
        {
            return (from stat in stats.Split(',')
                where stat.Split('#').Length == 5
                select new ItemStats
                {
                    Header = (StatsManager.Effect) Algorithm.HexToDeci(stat.Split('#')[0]),
                    MinValue = Algorithm.HexToDeci(stat.Split('#')[1]),
                    MaxValue = Algorithm.HexToDeci(stat.Split('#')[2]),
                    JetDecimal = stat.Split('#')[4]
                }).ToList();
        }
    }
}