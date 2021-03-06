﻿namespace SilverGame.Models.Items.Items
{
    class GeneralStats
    {
        public int Base { get; set; }
        public int Items { get; set; }
        public int Donation { get; set; }
        public int Boosts { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}",
                Base, Items, Donation, Boosts);
        }

        public int GetTotal()
        {
            return Base + Items + Donation + Boosts;
        }
    }
}
