using System;
using System.Collections.Generic;
using SilverSock;

namespace SilverGame.Network.Game
{
    class GameServer : Abstract.Server
    {
        List<GameClient> _clients;

        public GameServer()
            : base(Services.Config.get("Game_ip"), Int32.Parse(Services.Config.get("Game_port")))
        {
            _clients = new List<GameClient>();
        }

        public override void OnListening()
        {
            throw new NotImplementedException();
        }

        public override void OnListeningFailed(Exception e)
        {
            throw new NotImplementedException();
        }

        public override void OnSocketAccepted(SilverSocket socket)
        {
            throw new NotImplementedException();
        }
    }
}
