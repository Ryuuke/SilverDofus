using System;
using System.Collections.Generic;
using SilverGame.Services;
using SilverSock;

namespace SilverGame.Network.Game
{
    sealed class GameServer : Abstract.Server
    {
        public static List<GameClient> Clients;
        public static Object Lock = new object();

        public GameServer()
            : base(Config.Get("Game_ip"), Int32.Parse(Config.Get("Game_port")))
        {
            Clients = new List<GameClient>();
        }

        protected override void OnListening()
        {
            Console.WriteLine("Waiting for new connection ...");

            Logs.LogWritter(Constant.GameFolder, "GameServer Waiting for new connection");
        }

        protected override void OnListeningFailed(Exception e)
        {
            Console.WriteLine("Listening Failed, check your port or IP address ...");
            Logs.LogWritter(Constant.GameFolder, string.Format("GameServer listening failed {0}", e.Message));
        }

        protected override void OnSocketAccepted(SilverSocket socket)
        {
            Console.WriteLine("Connection With host " + socket.IP + " Successfuly");

            Logs.LogWritter(Constant.GameFolder, "GameServer Connection With host " + socket.IP + " Successfuly");

            lock (Lock)
                Clients.Add(new GameClient(socket));
        }
    }
}
