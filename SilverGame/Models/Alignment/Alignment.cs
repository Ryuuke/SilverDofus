namespace SilverGame.Models.Alignment
{
    class Alignment
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Honor { get; set; }
        public int Deshonor { get; set; }
        public int Level { get; set; }
        public int Grade { get; set; }
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return string.Format("{0}~1,{1},{2},{3},{4},{5}",
                Id, Level, Grade, Honor, Deshonor, Enabled);
        }
    }
}
