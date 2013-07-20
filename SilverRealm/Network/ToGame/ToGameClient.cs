using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverRealm.Network.ToGame
{
    class ToGameClient : Abstract.Client
    {
        private CommunicationState communicationState;

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
            lock (ToGameServer._lock)
                ToGameServer.games.Remove(this);

            Console.WriteLine("Connection closed with Game Server {0}", socket.IP);
        }

        public override void sendPackets(string packet)
        {
            base.sendPackets(packet);
        }

        public override void dataReceived(string packet)
        {
            switch (communicationState)
            {
                case CommunicationState.verifyGame :
                    VerifyGameServer(packet);
                    break;
            }
        }

        private void VerifyGameServer(string packet)
        {
            string key = packet.Substring(2);

            if (Database.GameServerRepository.getAll().Any((x) => x.key == key))
            {
                
            }
            else
            {
                Console.WriteLine("Cloud not find Game Server with key : {0}", key);
                this.OnSocketClosed();
            }
        }

        private enum CommunicationState
        {
            verifyGame,
        }
    }
}
