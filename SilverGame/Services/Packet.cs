namespace SilverGame.Services
{
    static class Packet
    {
        // packet for communication Realm << >> Game
        public const string
            HelloRealm = "HR",
            DisconnectMe = "DM";

        public const string
            HelloGameServer = "HG",
            TicketResponse = "AT",
            TicketExpired = "M130",
            InvalidIp = "M031",
            TicketAccepted = "ATK0",
            RegionalVersion = "AV",
            CharactersList = "AL",
            CharactersListResponse = "ALK",
            CharacterNameGenerated = "AP",
            CharacterNameGeneratedResponse = "APK",
            CharacterAdd = "AA",
            CreateCharacterBadName = "AAEn",
            NameAlredyExists = "AAEa",
            CreateCharacterFullErrors = "AAEf",
            CreationSuccess = "AAK",
            GameBegin = "TB",
            GiftsList = "Ag",
            GiftStored = "AG",
            GiftStotedSuccess = "AGK",
            CharacterSelected = "AS",
            CharacterSelectedResponse = "ASK",
            GameCreated = "GC",
            GameCreatedResponse = "GCK",
            Stats = "As",
            MapData = "GDM",
            MapLoaded = "GDK",
            CellData = "GDC",
            GameInfos = "GI",
            Restriction = "AR6bk",
            Movement = "GM",
            GameActions = "GA",
            GameEndAction = "GK",
            Nothing = "BN",
            ObjectMove = "OM",
            ObjectWeight = "Ow",
            ObjectQuantity = "OQ",
            ObjectDrop = "OD",
            ObjectAdd = "OAKO",
            ObjectRemove = "OR",
            ObjectItemSet = "OS",
            ObjectAccessories = "Oa",
            Date = "BD",
            ReferenceTime = "BT",
            SubscribeChannel = "cC",
            ServerMessage = "BM",
            DefaultMessage = "cMK",
            RecruitmentMessage = "cMK?",
            BusinessMessage = "cMK:",
            PrivateMessageReceiver = "cMKF",
            PrivateMessageSender = "cMKT",
            PrivateMessageNotConnectedReceiverError = "cMEF",
            AdminMesage = "cMK@",
            Message = "Im",
            StatsBoost = "AB";

        // Message Id (Im)
        public const string
            WelcomeToDofus = "189",
            LastConnectionInfos = "0152";
    }
}