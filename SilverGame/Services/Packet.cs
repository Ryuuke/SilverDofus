namespace SilverGame.Services
{
    static class Packet
    {
        // packet for communication Realm << >> Game
        public const string
            HelloRealm = "HR";

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
            CreationSuccess = "AAK";
    }
}