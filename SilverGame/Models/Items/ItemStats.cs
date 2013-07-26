using SilverGame.Services;

namespace SilverGame.Models.Items
{
    class ItemStats
    {
        public int Header { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string JetDecimal = "0d0+0";

        public override string ToString()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}",
                Algorithm.DeciToHex(Header),
                Algorithm.DeciToHex(MinValue),
                Algorithm.DeciToHex(MaxValue),
                "0",
                JetDecimal);
        }
    }
}
