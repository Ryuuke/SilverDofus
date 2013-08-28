using System;
using System.Linq;
using System.Text;
using System.Threading;
using SilverGame.Network.Game;
using SilverGame.Services;
using SilverSock;

namespace SilverGame.Network.ToRealm
{
    sealed class ToRealmClient
    {
        public static SilverSocket Socket ;

        public ToRealmClient()
        {
            Socket = new SilverSocket();
            {
                Socket.OnConnected += OnConnected;
                Socket.OnDataArrivalEvent += DataArrival;
                Socket.OnFailedToConnect += OnFailedToConnect;
                Socket.OnSocketClosedEvent += OnSocketClosed;
            }
        }

        public void ConnectToRealm()
        {
            SilverConsole.WriteLine("Com : Connection to Realm server...", ConsoleColor.DarkGreen);
            Logs.LogWritter(Constant.ComFolder, "Connection to Realm server...");
            Socket.ConnectTo(Config.Get("Realm_ip"), Int32.Parse(Config.Get("Com_port")));
        }

        public void OnConnected()
        {
            SilverConsole.WriteLine("Com : Connected" +
                                    " to Realm Server Successfully", ConsoleColor.Green);
            Logs.LogWritter(Constant.ComFolder, "Com : Connected to Realm Server Successfully");

            Thread.Sleep(500);

            SendPackets(string.Format("{0}{1}", Packet.HelloRealm, Config.Get("Game_key")));
        }

        public void OnFailedToConnect(Exception e)
        {
            SilverConsole.WriteLine("Com : Failed to connect to Realm Server", ConsoleColor.Red);
            Logs.LogWritter(Constant.ComFolder, "Com : Failed to connect to Realm Server");
            RetryToConnect();
        }

        public void OnSocketClosed()
        {
            SilverConsole.WriteLine("Com : Connection To Realm Server closed", ConsoleColor.Yellow);
            Logs.LogWritter(Constant.ComFolder, "Com : Connection to Realm Server closed");

            RetryToConnect();
        }

        private void RetryToConnect()
        {
            Thread.Sleep(Constant.TimeIntervalToReconnectToRealmServer);

            ConnectToRealm();
        }

        public static void SendPackets(string packet)
        {
            SilverConsole.WriteLine(string.Format("send >>" + string.Format("{0}\x00", packet)), ConsoleColor.Cyan);
            Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", packet)));
        }

        private void DataArrival(byte[] data)
        {
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                SilverConsole.WriteLine(string.Format("Recv <<" + packet), ConsoleColor.Green);
                DataReceived(packet);
            }
        }

        public void DataReceived(string packet)
        {
            switch (packet.Substring(0,2))
            {
                case Packet.DisconnectMe:
                    DisconectGameClient(packet.Substring(2));
                    break;
            }
        }

        private void DisconectGameClient(string id)
        {
            if (GameServer.Clients.All(x => x.Account.Id != int.Parse(id)))
                return;

            lock (GameServer.Lock)
                GameServer.Clients.Find(x => x.Account.Id == int.Parse(id)).OnSocketClosed();

            Database.Repository.AccountRepository.UpdateAccount(int.Parse(id), false);
        }
    }
}