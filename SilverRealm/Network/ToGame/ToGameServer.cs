using System;
using System.Collections.Generic;
using SilverRealm.Services;
using SilverSock;

namespace SilverRealm.Network.ToGame
{
    sealed class ToGameServer : Abstract.Server
    {
        public static List<ToGameClient> Games;
        public static Object Lock = new Object();

        public ToGameServer()
            : base(Services.Config.Get("Realm_ip"), Int32.Parse(Services.Config.Get("port_com")))
        {
            Games = new List<ToGameClient>();
        }

        protected override void OnListening()
        {
            Console.WriteLine("Waiting for connection to game servers ...");
            Logs.LogWritter(Constant.ComFolder, "Waiting for connection to game servers ...");
        }

        protected override void OnListeningFailed(Exception e)
        {
            Console.WriteLine("Failed listening server communication verify ip_com or port_com");
            Logs.LogWritter(Constant.ErrorsFolder, string.Format("ComServer {0}", e.Message));
        }

        protected override void OnSocketAccepted(SilverSocket socket)
        {
            Console.WriteLine("Connection successfuly with game server " + socket.IP);

            Logs.LogWritter(Constant.ComFolder, string.Format("Connection successfuly with game server {0}", socket.IP));

            lock (Lock)
                Games.Add(new ToGameClient(socket));
        }
    }
}
