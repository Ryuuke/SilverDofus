namespace SilverRealm.Services
{
    class Packet
    {
        public const string
            HelloConnectionServer = "HC",
            WrongDofusVersion = "AlEv",
            WrongDofusAccount = "AlEf",
            BannedAccount = "AlEb",
            BannedTime = "AlEk",
            AlredyConnected = "AlEa",
            DofusPseudo = "Ad",
            Community = "Ac",
            Hosts = "AH",
            IsAdmin = "AlK",
            SecretQuestion = "AQ",
            NewQueue = "Af",
            ServersList = "Ax",
            SubscriptionPlayerList = "AxK",
            FriendServerList = "AF",
            SelectServer = "AX",
            ConnectToGameServer = "AYK",
            UnavailableServer = "AXEd";


        // packet for communication Realm << >> Game
        public const string
                HelloRealm = "HR",
                NumberOfPlayers = "NoP";
    }
}