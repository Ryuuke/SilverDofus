using System;
using System.Linq;
using System.Text;
using SilverRealm.Network.Realm;
using SilverRealm.Services;
using SilverSock;

namespace SilverRealm.Network.ToGame
{
    sealed class ToGameClient
    {
        public static SilverSocket Socket;

        private readonly CommunicationState _communicationState;
        private string _key;

        public ToGameClient(SilverSocket socket)
        {
            Socket = socket;
            {
                socket.OnDataArrivalEvent += DataArrival;
                socket.OnSocketClosedEvent += OnSocketClosed;
            }

            _communicationState = CommunicationState.VerifyGame;
        }

        public void OnSocketClosed()
        {

            RealmClient.GameServers.Single(gameServer => gameServer.ServerKey == _key).State = 0;      

            lock (RealmServer.Lock)
            {
                foreach (var client in RealmServer.Clients)
                {
                    client.RefreshServerList();
                }
            }

            SilverConsole.WriteLine(string.Format("Connection closed with Game Server {0}", Socket.IP), ConsoleColor.Yellow);
            Logs.LogWritter(Constant.ComFolder, string.Format("Connection closed with Game Server {0}", Socket.IP));

            lock (ToGameServer.Lock)
                ToGameServer.Games.Remove(this);
        }

        public static void SendPacket(string packet)
        {
            SilverConsole.WriteLine(string.Format("send >>" + string.Format("{0}\x00", packet)), ConsoleColor.Cyan);
            Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", packet)));
        }

        public void DataArrival(byte[] data)
        {
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                SilverConsole.WriteLine("Recv <<" + packet, ConsoleColor.Green);
                Logs.LogWritter(Constant.ComFolder, string.Format("Recv << {0}", packet));
                DataReceived(packet);
            }
        }

        public void DataReceived(string packet)
        {
            switch (_communicationState)
            {
                case CommunicationState.VerifyGame :
                    VerifyGameServer(packet);
                    break;
            }
        }

        private void VerifyGameServer(string packet)
        {
            _key = packet.Substring(2);

            RealmClient.GameServers.Single(gameServer => gameServer.ServerKey == _key).State = 1;

            lock (RealmServer.Lock)
            {
                foreach (var client in RealmServer.Clients)
                {
                    client.RefreshServerList();
                }
            }
        }

        private enum CommunicationState
        {
            VerifyGame,
        }
    }
}