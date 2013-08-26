namespace SilverRealm.Services
{
    static class Constant
    {
        public const string ConfigFile = "RealmConfig.txt";
        public const string Version = "1.29.1"; // version actuel du client
        public const long OneYear = 31536000000;
        public const short DiscoveryMode = 0;
        public const string PseudoColumnName = "pseudo";
        public const string UsernameColumnName = "username";
        public const string ServerListFormat = "|{0},{1}";
        public const string FriendsServerListFormat = "{0},{1};";

        public const string
            ErrorsFolder = "Exceptions_logs",
            ComFolder = "Com_logs",
            RealmFolder = "Realm_logs";
    }
}
