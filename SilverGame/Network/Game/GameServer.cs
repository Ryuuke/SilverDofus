using System;
using System.Collections.Generic;
using SilverGame.Network.Abstract;
using SilverGame.Services;
using SilverSock;

namespace SilverGame.Network.Game
{
    sealed class GameServer : Server
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
            SilverConsole.WriteLine("Waiting for new connection ...", ConsoleColor.DarkGreen);

            Logs.LogWritter(Constant.GameFolder, "GameServer Waiting for new connection");
        }

        protected override void OnListeningFailed(Exception e)
        {
            SilverConsole.WriteLine("Error : Listening Failed, check your port or IP address ...", ConsoleColor.Red);
            Logs.LogWritter(Constant.GameFolder, string.Format("GameServer listening failed {0}", e.Message));
        }

        protected override void OnSocketAccepted(SilverSocket socket)
        {
            SilverConsole.WriteLine("Connection With host " + socket.IP + " Successfuly", ConsoleColor.Green);

            Logs.LogWritter(Constant.GameFolder, "GameServer Connection With host " + socket.IP + " Successfuly");

            lock (Lock)
                Clients.Add(new GameClient(socket));

            Console.WriteLine(GameServer.Clients.Count);
        }
    }
}
