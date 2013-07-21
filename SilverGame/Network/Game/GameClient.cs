using System;
using SilverSock;

namespace SilverGame.Network.Game
{
    class GameClient : Abstract.Client
    {
        public GameClient(SilverSocket socket)
            : base(socket)
        {

        }

        public override void OnConnected()
        {
            throw new NotImplementedException();
        }

        public override void OnFailedToConnect(Exception e)
        {
            throw new NotImplementedException();
        }

        public override void OnSocketClosed()
        {
            throw new NotImplementedException();
        }

        public override void DataReceived(string packet)
        {
            throw new NotImplementedException();
        }
    }
}
