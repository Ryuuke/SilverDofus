using System;
using System.Linq;
using SilverRealm.Network.Realm;
using SilverRealm.Services;
using SilverRealm.Network.Abstract;
using SilverSock;

namespace SilverRealm.Network.ToGame
{
    sealed class ToGameClient : Client
    {
        private readonly CommunicationState _communicationState;
        private string _key;

        public ToGameClient(SilverSocket socket)
            : base(socket)
        {
            _communicationState = CommunicationState.VerifyGame;
        }

        protected override void OnConnected()
        {
           
        }

        protected override void OnFailedToConnect(Exception e)
        {
           
        }

        protected override void OnSocketClosed()
        {
            try
            {
                RealmClient.GameServers.Single(gameServer => gameServer.ServerKey == _key).State = 0;
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.Message);
            }
            

            lock (RealmServer.Lock)
            {
                foreach (var client in RealmServer.Clients)
                {
                    client.RefreshServerList();
                }
            }

            SilverConsole.WriteLine(string.Format("Connection closed with Game Server {0}", Socket.IP), ConsoleColor.Yellow);
            Logs.LogWritter(Constant.ComFolder, string.Format("Connection closed with Game Server {0}", Socket.IP));

            lock (ToGameServer.Lock)
                ToGameServer.Games.Remove(this);
        }

        protected override void DataReceived(string packet)
        {
            Logs.LogWritter(Constant.ComFolder, string.Format("Recv << {0}", packet));

            switch (_communicationState)
            {
                case CommunicationState.VerifyGame :
                    VerifyGameServer(packet);
                    break;
            }
        }

        private void VerifyGameServer(string packet)
        {
            _key = packet.Substring(2);

            RealmClient.GameServers.Single(gameServer => gameServer.ServerKey == _key).State = 1;

            lock (RealmServer.Lock)
            {
                foreach (var client in RealmServer.Clients)
                {
                    client.RefreshServerList();
                }
            }
        }

        private enum CommunicationState
        {
            VerifyGame,
        }
    }
}