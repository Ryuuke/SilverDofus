using SilverGame.Models.Items.Items;
using SilverGame.Services;

namespace SilverGame.Models.Items.Abstract
{
    class Jet
    {
        public StatsManager.Effect Header { get; set; }
        public int MinValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0}#{1}#0#0", Algorithm.DeciToHex((int) Header), Algorithm.DeciToHex(MinValue));
        }
    }
}
