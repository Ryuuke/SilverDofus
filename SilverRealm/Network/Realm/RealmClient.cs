using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SilverRealm.Models;
using SilverRealm.Network.Abstract;
using SilverSock;
using SilverRealm.Services;

namespace SilverRealm.Network.Realm
{
    sealed class RealmClient : Client
    {
        private readonly string _key;

        private StateConnecion _stateConnxion;
        public Account Account;

        public static List<GameServer> GameServers;

        public RealmClient(SilverSocket socket)
            : base(socket)
        {
            _key = Hash.RandomString(32);

            SendPackets(string.Format("{0}{1}", Packet.HelloConnectionServer, _key));
        }

        protected override void OnConnected()
        {
            
        }

        protected override void OnFailedToConnect(Exception e)
        {
            Logs.LogWritter(Constant.ErrorsFolder, _stateConnxion == StateConnecion.CheckingServer
                 ? string.Format("{0}:ip {1} {2}", Account.Username, Socket.IP, e.Message)
                 : string.Format("ip {0} {1}", Socket.IP, e.Message));
        }

        protected override void OnSocketClosed()
        {
            SilverConsole.WriteLine("Connection closed", ConsoleColor.Yellow);
            Logs.LogWritter(Constant.RealmFolder, _stateConnxion == StateConnecion.CheckingServer
                ? string.Format("{0}:ip {1}  Connection closed", Account.Username, Socket.IP)
                : string.Format("ip {0} Connection closed", Socket.IP));

            lock (RealmServer.Lock)
                RealmServer.Clients.Remove(this);
        }

        protected override void SendPackets(string packet)
        {
            base.SendPackets(packet);

            Logs.LogWritter(Constant.RealmFolder, _stateConnxion == StateConnecion.CheckingServer
                ? string.Format("{0}:ip {1} Send >> {2}", Account.Username, Socket.IP, packet)
                : string.Format("ip {0} Send >> {1}", Socket.IP, packet));
        }

        protected override void DataReceived(string packet)
        {
            Logs.LogWritter(Constant.RealmFolder, _stateConnxion == StateConnecion.CheckingServer 
                ? string.Format("{0}:ip {1} Recv << {2}", Account.Username, Socket.IP, packet)
                : string.Format("ip {0} Recv << {1}", Socket.IP, packet));

            switch (_stateConnxion)
            {
                case StateConnecion.CheckingVersion:
                    _stateConnxion = StateConnecion.CheckingAccount;
                    CheckVersion(packet);
                    break;

                case StateConnecion.CheckingAccount:
                    _stateConnxion = StateConnecion.CheckingServer;
                    CheckAccount(packet);
                    break;

                case StateConnecion.CheckingServer:
                    CheckQueue(packet);
                    break;
            }
        }

        private void CheckVersion(string packet)
        {
            if (Constant.Version == packet) return;
                SendPackets(Packet.WrongDofusVersion);
                OnSocketClosed();
        }

        private void CheckAccount(string packet)
        {
            var username = packet.Split('#')[0];
            var password = packet.Split('#')[1];

            Account = Database.AccountRepository.GetAccount(Constant.UsernameColumnName, username);

            if (Account == null || Hash.Encrypt(Account.Password, _key) != password)
            {
                SendPackets(Packet.WrongDofusAccount);
                OnSocketClosed();
            }
            else if (RealmServer.Clients.Count(x => x.Account.Username == username) > 1 || Account.Connected)
            {
                SendPackets(Packet.AlredyConnected);
                OnSocketClosed();
            }
            else if (Account.BannedUntil != null && Account.BannedUntil > DateTime.Now)
            {
                SendPackets(Packet.BannedAccount);
                OnSocketClosed();
            }
            else
            {
                SendPackets(string.Format("{0}{1}", Packet.DofusPseudo, Account.Pseudo));
                SendPackets(string.Format("{0}{1}", Packet.Community, 0)); // 0 : communauté fr

                RefreshServerList();

                SendPackets(string.Format("{0}{1}", Packet.IsAdmin, Account.GmLevel > 0 ? 1 : 0));
                SendPackets(string.Format("{0}{1}", Packet.SecretQuestion, Account.Question.Replace(" ", "+")));
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
                        (Account.Subscription == null || DateTime.Now >= Account.Subscription.Value)
                            ? (object) Constant.DiscoveryMode
                            : (Account.Subscription.Value - DateTime.Now).TotalMilliseconds.ToString(CultureInfo.InvariantCulture).Split('.')[0], 
                    listCharactersByGameServer));
            }
            else
            {
                SendPackets(string.Format("{0}{1}{2}", Packet.SubscriptionPlayerList, Constant.OneYear, listCharactersByGameServer));
            }
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
            var gameServerId = Int32.Parse(packet.Substring(2));

            var gameServer = GameServers.Single(gS => gS.Id == gameServerId);

            if (gameServer == null)
                return;

            SendPackets(gameServer.State == 0
                ? Packet.UnavailableServer
                : string.Format("{0}{1}:{2};{3}", Packet.ConnectToGameServer, gameServer.Ip, gameServer.Port,
                    Hash.GenerateTicketKey(Socket, Account)));
        }

        public enum StateConnecion
        {
            CheckingVersion,
            CheckingAccount,
            CheckingServer,
        }
    }
}