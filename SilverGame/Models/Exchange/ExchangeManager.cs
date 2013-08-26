using System.Collections.Generic;
using SilverGame.Models.Characters;

namespace SilverGame.Models.Exchange
{
    static class ExchangeManager
    {
        public readonly static List<Exchange> Exchanges = new List<Exchange>();

        public static void CreateExchangeSession(Character senderCharacter, Character receiverCharacter)
        {
            lock (Exchanges)
                Exchanges.Add(new Exchange (senderCharacter, receiverCharacter));
        }

        public static void CloseExchangeSession(Exchange exchange)
        {
            lock (Exchanges)
                Exchanges.Remove(exchange);
        }
    }
}
