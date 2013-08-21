namespace SilverGame.Services
{
    static class Constant
    {
        public const string ConfigFile = "GameConfig.txt";
        public const long OneYear = 31536000000;
        public const int DiscoveryMode = 0;
        public const int TicketTimeExpiredInterval = 10;
        public const int TimeIntervalToReconnectToRealmServer = 10000;

        public const string 
            ErrorsFolder = "Exceptions_logs",
            ComFolder = "Com_logs",
            GameFolder = "Game_logs";

        // Level
        public const int HighLevel = 100;

        // stats
        public const int BasePa = 6,
            BaseHighLevelPa = 7,
            BasePm = 3,
            BaseProspection = 100,
            BaseProspectionEnu = 120,
            BaseWeight = 1000;
    }
}
