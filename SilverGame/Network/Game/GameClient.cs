using System;
using SilverSock;
using SilverGame.Services;

namespace SilverGame.Network.Game
{
    sealed class GameClient : Abstract.Client
    {
        private GameState _gameState;

        public GameClient(SilverSocket socket)
            : base(socket)
        {
            SendPackets(Packet.HelloGameServer);
        }

        protected override void OnConnected()
        {
            
        }

        protected override void OnFailedToConnect(Exception e)
        {
            
        }

        protected override void OnSocketClosed()
        {
            
        }

        protected override void DataReceived(string packet)
        {
            
        }

        public enum GameState
        {
            ReceiveTicket,
        }
    }
}
