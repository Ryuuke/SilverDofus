using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override void sendPackets(string packet)
        {
            base.sendPackets(packet);
        }

        public override void dataReceived(string packet)
        {
            throw new NotImplementedException();
        }
    }
}
