using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverGame.Network.Game
{
    class GameServer : Abstract.Server
    {
        List<GameClient> clients;

        public GameServer()
            : base(Services.Config.get("Game_ip"), Int32.Parse(Services.Config.get("Game_port")))
        {
            clients = new List<GameClient>();
        }

        public override void onListening()
        {
            throw new NotImplementedException();
        }

        public override void onListeningFailed(Exception e)
        {
            throw new NotImplementedException();
        }

        public override void onSocketAccepted(SilverSocket socket)
        {
            throw new NotImplementedException();
        }
    }
}
