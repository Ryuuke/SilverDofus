namespace SilverGame.Models.Maps
{
    class MapTrigger
    {
        public Map Map { get; set; }
        public int Cell { get; set; }
        public Map NewMap { get; set; }
        public int NewCell { get; set; }
    }
}
