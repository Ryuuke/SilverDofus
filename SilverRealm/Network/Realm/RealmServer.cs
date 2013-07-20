using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverRealm.Network.Realm
{
    class RealmServer : Abstract.Server
    {
        public static List<RealmClient> Clients;
        public static Object _lock = new Object();

        public RealmServer()
            : base(Services.Config.get("Realm_ip"), Int32.Parse(Services.Config.get("Realm_port")))
        {
            Clients = new List<RealmClient>();
        }

        public override void onSocketAccepted(SilverSocket socket)
        {
            Console.WriteLine("Connection established");

            lock (_lock)
                Clients.Add(new RealmClient(socket));
        }

        public override void onListening()
        {
            Console.WriteLine("Waiting for new Connexions");
        }

        public override void onListeningFailed(Exception e)
        {
            Console.WriteLine("Listening Failed : " + e.Message);
        }
    }
}
