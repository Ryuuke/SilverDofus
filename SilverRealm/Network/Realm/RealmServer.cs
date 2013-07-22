using System;
using System.Collections.Generic;
using SilverSock;

namespace SilverRealm.Network.Realm
{
    sealed class RealmServer : Abstract.Server
    {
        public static List<RealmClient> Clients;
        public static Object Lock = new Object();

        public RealmServer()
            : base(Services.Config.Get("Realm_ip"), Int32.Parse(Services.Config.Get("Realm_port")))
        {
            Clients = new List<RealmClient>();
        }

        protected override void OnSocketAccepted(SilverSocket socket)
        {
            Console.WriteLine("Connection established");

            lock (Lock)
                Clients.Add(new RealmClient(socket));
        }

        protected override void OnListening()
        {
            Console.WriteLine("Waiting for new Connexions");
        }

        protected override void OnListeningFailed(Exception e)
        {
            Console.WriteLine("Listening Failed : " + e.Message);
        }
    }
}
