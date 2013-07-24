using System;
using SilverGame.Models;
using SilverGame.Network.Abstract;
using SilverGame.Services;
using SilverSock;

namespace SilverGame.Network.Game
{
    sealed class GameClient : Client
    {
        private readonly GameParser.GameParser _parser;
        public Account Account;

        public GameClient(SilverSocket socket)
            : base(socket)
        {
            _parser = new GameParser.GameParser(this);
        }

        protected override void OnConnected()
        {
            
        }

        protected override void OnFailedToConnect(Exception e)
        {
            
        }

        public override void OnSocketClosed()
        {
            SilverConsole.WriteLine("Connection closed", ConsoleColor.Yellow);

            Logs.LogWritter(Constant.GameFolder, Account != null
                ? string.Format("{0}:ip {1} Connection Closed", Account.Username, Socket.IP)
                : string.Format("ip {0} Connection Closed", Socket.IP));

            lock (GameServer.Lock)
            GameServer.Clients.Remove(this);
        }

        public override void SendPackets(string packet)
        {
            base.SendPackets(packet);

            Logs.LogWritter(Constant.GameFolder, Account != null
                ? string.Format("{0}:ip {1} Send >> {2}", Account.Username, Socket.IP, packet)
                : string.Format("ip {0} Send >> {1}", Socket.IP, packet));
        }

        protected override void DataReceived(string packet)
        {
            Logs.LogWritter(Constant.GameFolder, Account != null
                ? string.Format("{0}:ip {1} Recv << {2}", Account.Username, Socket.IP, packet)
                : string.Format("ip {0} Recv << {1}", Socket.IP, packet));

            _parser.Parse(packet);
        }
    }
}