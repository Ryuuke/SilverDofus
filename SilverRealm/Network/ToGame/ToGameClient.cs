using System;
using System.Linq;
using SilverRealm.Properties;
using SilverSock;

namespace SilverRealm.Network.ToGame
{
    sealed class ToGameClient : Abstract.Client
    {
        [UsedImplicitly] private CommunicationState _communicationState;

        public ToGameClient(SilverSocket socket)
            : base(socket)
        {

        }

        public override void OnConnected()
        {
            
        }

        public override void OnFailedToConnect(Exception e)
        {
           
        }

        public override void OnSocketClosed()
        {
            lock (ToGameServer.Lock)
                ToGameServer.Games.Remove(this);

            Console.WriteLine("Connection closed with Game Server {0}", Socket.IP);
        }

        public override void DataReceived(string packet)
        {
            switch (_communicationState)
            {
                case CommunicationState.VerifyGame :
                    VerifyGameServer(packet);
                    break;
            }
        }

        private void VerifyGameServer(string packet)
        {
            var key = packet.Substring(2);

            if (Database.GameServerRepository.GetAll().Any(x => x.Key == key))
            {
                
            }
            else
            {
                Console.WriteLine("Cloud not find Game Server with key : {0}", key);
                OnSocketClosed();
            }
        }

        private enum CommunicationState
        {
            VerifyGame,
        }
    }
}
