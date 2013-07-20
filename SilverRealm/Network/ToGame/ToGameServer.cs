using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverRealm.Network.ToGame
{

    class ToGameServer : Abstract.Server
    {
        public static List<ToGameClient> games;
        public static Object _lock = new Object();

        public ToGameServer()
            : base(Services.Config.get("Realm_ip"), Int32.Parse(Services.Config.get("port_com")))
        {
            games = new List<ToGameClient>();
        }

        public override void onListening()
        {
            Console.WriteLine("Waiting for connection to game servers ...");
        }

        public override void onListeningFailed(Exception e)
        {
            Console.WriteLine("Failed listening server communication verify ip_com or port_com");
        }

        public override void onSocketAccepted(SilverSocket socket)
        {
            lock (_lock)
                games.Add(new ToGameClient(socket));
        }
    }
}
