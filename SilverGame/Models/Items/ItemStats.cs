using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Services;

namespace SilverGame.Models.Items
{
    class ItemStats
    {
        public StatsManager.Effect Header { get; set; }
        public int MinValue { get; set; }
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

        public static List<ItemStats> GenerateRandomStats(IEnumerable<ItemStats> stats)
        {
            var rand = new Random();

            var generatedStats = stats.Select(itemStats => new ItemStats
            {
                Header = itemStats.Header,
                MinValue =
                    itemStats.MaxValue == 0 ? itemStats.MinValue : rand.Next(itemStats.MinValue, itemStats.MaxValue),
                MaxValue = 0,
                JetDecimal = itemStats.JetDecimal,
            }).ToList();

            return generatedStats;
        }

        public static List<ItemStats> ToStats(string stats)
        {
            var listStats = (from stat in stats.Split(',')
                where stat.Split('#').Length == 5
                select new ItemStats
                {
                    Header = (StatsManager.Effect) Algorithm.HexToDeci(stat.Split('#')[0]),
                    MinValue = Algorithm.HexToDeci(stat.Split('#')[1]),
                    MaxValue = Algorithm.HexToDeci(stat.Split('#')[2]),
                    JetDecimal = stat.Split('#')[4]
                }).ToList();

            return listStats;
        }
    }
}