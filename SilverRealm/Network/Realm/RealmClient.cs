using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SilverRealm.Models;
using SilverRealm.Network.ToGame;
using SilverSock;
using SilverRealm.Services;

namespace SilverRealm.Network.Realm
{
    sealed class RealmClient
    {
        public readonly SilverSocket Socket;

        private readonly string _key;

        private StateConnecion _stateConnxion;
        public Account Account;

        public static List<GameServer> GameServers;

        public RealmClient(SilverSocket socket)
        {
            Socket = socket;
            {
                socket.OnDataArrivalEvent += DataArrival;
                socket.OnSocketClosedEvent += OnSocketClosed;
            }

            _key = Hash.RandomString(32);

            SendPackets(string.Format("{0}{1}", Packet.HelloConnectionServer, _key));
        }

        private void OnSocketClosed()
        {
            SilverConsole.WriteLine("Connection closed", ConsoleColor.Yellow);
            Logs.LogWritter(Constant.RealmFolder, _stateConnxion == StateConnecion.CheckingServer
                ? string.Format("{0}:ip {1}  Connection closed", Account.Username, Socket.IP)
                : string.Format("ip {0} Connection closed", Socket.IP));

            RemoveMeOnList();
        }

        public void RemoveMeOnList()
        {
            lock (RealmServer.Lock)
                RealmServer.Clients.Remove(this);
        }

        public void SendPackets(string packet)
        {
            SilverConsole.WriteLine(string.Format("send >>" + string.Format("{0}\x00", packet)), ConsoleColor.Cyan);
            Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", packet)));

            Logs.LogWritter(Constant.RealmFolder, _stateConnxion == StateConnecion.CheckingServer
                ? string.Format("{0}:ip {1} Send >> {2}", Account.Username, Socket.IP, packet)
                : string.Format("ip {0} Send >> {1}", Socket.IP, packet));
        }

        public void DataArrival(byte[] data)
        {
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                SilverConsole.WriteLine("Recv <<" + packet, ConsoleColor.Green);

                Logs.LogWritter(Constant.RealmFolder, _stateConnxion == StateConnecion.CheckingServer
                ? string.Format("{0}:ip {1} Recv << {2}", Account.Username, Socket.IP, packet)
                : string.Format("ip {0} Recv << {1}", Socket.IP, packet));

                DataReceived(packet);
            }
        }

        private void DataReceived(string packet)
        {
            switch (_stateConnxion)
            {
                case StateConnecion.CheckingVersion:
                    CheckVersion(packet);
                    break;

                case StateConnecion.CheckingAccount:
                    CheckAccount(packet);
                    break;

                case StateConnecion.CheckingServer:
                    CheckQueue(packet);
                    break;
            }
        }

        private void CheckVersion(string packet)
        {
            if (Constant.Version != packet)
            {
                SendPackets(Packet.WrongDofusVersion);
                RemoveMeOnList();
            }
            else
                _stateConnxion = StateConnecion.CheckingAccount;
        }

        private void CheckAccount(string packet)
        {
            var username = packet.Split('#')[0];
            var password = packet.Split('#')[1];

            Account = Database.AccountRepository.GetAccount(Constant.UsernameColumnName, username);

            if (Account == null || Hash.Encrypt(Account.Password, _key) != password)
            {
                SendPackets(Packet.WrongDofusAccount);
                RemoveMeOnList();
            }
            else if (RealmServer.Clients.Count(x => x.Account.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) > 1 || Account.Connected)
            {
                SendPackets(Packet.AlredyConnected);

                RealmServer.Disconnect(Account.Id);

                ToGameClient.SendPacket(string.Format("{0}{1}", Packet.DisconnectMe, Account.Id));

                RemoveMeOnList();
            }
            else if (Account.BannedUntil != null && Account.BannedUntil > DateTime.Now)
            {
                SendPackets(Packet.BannedAccount);
                RemoveMeOnList();
            }
            else
            {     
                SendPackets(string.Format("{0}{1}", Packet.DofusPseudo, Account.Pseudo));
                SendPackets(string.Format("{0}{1}", Packet.Community, 0)); // 0 : communauté fr

                RefreshServerList();

                SendPackets(string.Format("{0}{1}", Packet.IsAdmin, Account.GmLevel > 0 ? 1 : 0));
                SendPackets(string.Format("{0}{1}", Packet.SecretQuestion, Account.Question.Replace(" ", "+")));

                _stateConnxion = StateConnecion.CheckingServer;
            }
        }

        public void RefreshServerList()
        {
            SendPackets(string.Format("{0}{1}", Packet.Hosts, string.Join("|", GameServers)));
        }

        private void CheckQueue(string packet)
        {
            if (!packet.StartsWith("A"))
                return;

            switch (packet.Substring(0,2))
            {
                case Packet.NewQueue:
                    CheckQueue();
                    break;

                case Packet.ServersList:
                    SendServersList();
                    break;

                case Packet.FriendServerList:
                    SendFriendServerList(packet);
                    break;

                case Packet.SelectServer:
                    ConnectToGameServer(packet);
                    break;
            }
        }

        private void CheckQueue()
        {
            SendPackets(string.Format("{0}{1}", Packet.NewQueue, "|0|0|1|-1"));
        }

        private void SendServersList()
        {
            var listCharactersByGameServer = Database.Characters.GetCharactersByGameServer(Account.Id, Constant.ServerListFormat);

            if (bool.Parse(Config.Get("subscription")))
            {
                SendPackets(string.Format("{0}{1}{2}", Packet.SubscriptionPlayerList, 
                        Account.IsNotSubscribed()
                            ? (object) Constant.DiscoveryMode
                            : (Account.Subscription.Value - DateTime.Now).TotalMilliseconds.ToString(CultureInfo.InvariantCulture).Split('.')[0], 
                        listCharactersByGameServer));
            }
            else
                SendPackets(string.Format("{0}{1}{2}", Packet.SubscriptionPlayerList, Constant.OneYear, listCharactersByGameServer));
        }

        private void SendFriendServerList(string packet)
        {
            var pseudo = packet.Substring(2);

            var account = Database.AccountRepository.GetAccount(Constant.PseudoColumnName, pseudo);

            if (account == null)
                return;

            var friendsServerList = Database.Characters.GetCharactersByGameServer(account.Id, Constant.FriendsServerListFormat);

            SendPackets(string.Format("{0}{1}", Packet.FriendServerList, friendsServerList));
        }

        private void ConnectToGameServer(string packet)
        {
            var gameServerId = Int16.Parse(packet.Substring(2));

            var gameServer = GameServers.Single(gS => gS.Id == gameServerId);

            if (gameServer == null)
                return;

            SendPackets(
                gameServer.State == 0
                    ? Packet.UnavailableServer
                    : string.Format("{0}{1}:{2};{3}", Packet.ConnectToGameServer, gameServer.Ip, gameServer.Port,
                Hash.GenerateTicketKey(Socket, Account)));
        }

        public void Disconnect()
        {
            Socket.CloseSocket();

            RemoveMeOnList();
        }

        public enum StateConnecion
        {
            CheckingVersion,
            CheckingAccount,
            CheckingServer,
        }
    }
}