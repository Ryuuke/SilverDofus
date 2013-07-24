using System;
using System.Threading;
using SilverGame.Network.Abstract;
using SilverGame.Services;
using SilverSock;

namespace SilverGame.Network.ToRealm
{
    sealed class ToRealmClient : Client
    {
        public ToRealmClient()
            : base(new SilverSocket())
        {
            ConnectToRealm();
        }

        public void ConnectToRealm()
        {
            SilverConsole.WriteLine("Connection to Realm server...");
            Logs.LogWritter(Constant.ComFolder, "Connection to Realm server...");
            Socket.ConnectTo(Config.Get("Realm_ip"), Int32.Parse(Config.Get("Com_port")));
        }

        protected override void OnConnected()
        {
            SilverConsole.WriteLine("Com : Connected to Realm Server Successfully", ConsoleColor.Green);
            Logs.LogWritter(Constant.ComFolder, "Com : Connected to Realm Server Successfully");

            Thread.Sleep(500);

            SendPackets(string.Format("{0}{1}", Packet.HelloRealm, Config.Get("Game_key")));
        }

        protected override void OnFailedToConnect(Exception e)
        {
            SilverConsole.WriteLine("Com : Failed to connect to Realm Server", ConsoleColor.Red);
            Logs.LogWritter(Constant.ComFolder, "Com : Failed to connect to Realm Server");
            RetryToConnect();
        }

        public override void OnSocketClosed()
        {
            SilverConsole.WriteLine("Connection To Realm Server closed", ConsoleColor.Yellow);
            Logs.LogWritter(Constant.ComFolder, "Com : Connection to Realm Server closed");

            RetryToConnect();
        }

        private void RetryToConnect()
        {
            Thread.Sleep(Constant.TimeIntervalToReconnectToRealmServer);

            ConnectToRealm();
        }

        protected override void DataReceived(string packet)
        {
            throw new NotImplementedException();
        }
    }
}