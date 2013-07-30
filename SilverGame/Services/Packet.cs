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
            CreateCharacterFullErrors = "AAEF",
            CreationSuccess = "AAK",
            GiftsList = "Ag",
            GiftStored = "AG",
            GiftStotedSuccess = "AGK",
            CharacterSelected = "AS",
            CharacterSelectedResponse = "ASK",
            GameCreated = "GC",
            GameCreatedResponse = "GCK",
            Stats = "As";
    }
}