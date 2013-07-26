using System;
using System.Collections.Generic;
using SilverRealm.Services;
using SilverRealm.Network.Abstract;
using SilverSock;

namespace SilverRealm.Network.ToGame
{
    sealed class ToGameServer : Server
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
            SilverConsole.WriteLine("Waiting for connection to game servers ...", ConsoleColor.DarkGreen);
            Logs.LogWritter(Constant.ComFolder, "Waiting for connection to game servers ...");
        }

        protected override void OnListeningFailed(Exception e)
        {
            SilverConsole.WriteLine("Com : Failed listening server communication verify ip_com or port_com", ConsoleColor.Red);
            Logs.LogWritter(Constant.ErrorsFolder, string.Format("ComServer {0}", e.Message));
        }

        protected override void OnSocketAccepted(SilverSocket socket)
        {
            SilverConsole.WriteLine("Com : Connection successfuly with game server " + socket.IP, ConsoleColor.Green);

            Logs.LogWritter(Constant.ComFolder, string.Format("Connection successfuly with game server {0}", socket.IP));

            lock (Lock)
                Games.Add(new ToGameClient(socket));
        }
    }
}
