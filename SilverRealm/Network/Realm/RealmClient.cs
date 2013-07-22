using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SilverRealm.Models;
using SilverSock;
using SilverRealm.Services;

namespace SilverRealm.Network.Realm
{
    sealed class RealmClient : Abstract.Client
    {
        private readonly string _key;

        private StateConnecion _stateConnxion;
        private Account _account;

        public static List<GameServer> GameServers = Database.GameServerRepository.GetAll();

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
            Console.WriteLine("Failed to connect to client");
        }

        protected override void OnSocketClosed()
        {
            Console.WriteLine("Connection closed");

            lock (RealmServer.Lock)
                RealmServer.Clients.Remove(this);
        }

        protected override void DataReceived(string packet)
        {
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

            _account = Database.AccountRepository.GetAccount(Constant.UsernameColumnName, username);

            if (_account == null || Hash.Encrypt(_account.Password, _key) != password)
            {
                SendPackets(Packet.WrongDofusAccount);
                OnSocketClosed();
            }
            else if (RealmServer.Clients.Count(x => x._account.Username == username) > 1)
            {
                SendPackets(Packet.AlredyConnected);
                OnSocketClosed();
            }
            else if (_account.BannedUntil != null && _account.BannedUntil > DateTime.Now)
            {
                SendPackets(Packet.BannedAccount);
                OnSocketClosed();
            }
            else
            {
                SendPackets(string.Format("{0}{1}", Packet.DofusPseudo, _account.Pseudo));
                SendPackets(string.Format("{0}{1}", Packet.Community, 0)); // 0 : communauté fr

                RefreshServerList();

                SendPackets(string.Format("{0}{1}", Packet.IsAdmin, _account.GmLevel > 0 ? 1 : 0));
                SendPackets(string.Format("{0}{1}", Packet.SecretQuestion, _account.Question.Replace(" ", "+")));
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
            var listCharactersByGameServer = Database.Characters.GetCharactersByGameServer(_account.Id, Constant.ServerListFormat);

            if (bool.Parse(Config.Get("subscription")))
            {
                if (_account.Subscription == null || DateTime.Now >= _account.Subscription.Value)
                {
                    SendPackets(string.Format("{0}{1}{2}", Packet.SubscriptionPlayerList, Constant.DiscoveryMode, listCharactersByGameServer));
                }
                else
                {
                    SendPackets(string.Format("{0}{1}{2}", Packet.SubscriptionPlayerList, (_account.Subscription.Value - DateTime.Now).TotalMilliseconds.ToString(CultureInfo.InvariantCulture).Split(',')[0], listCharactersByGameServer));
                }
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

            var gameServer = GameServers.Single(x => x.Id == gameServerId);

            if (gameServer == null)
                return;

            SendPackets(gameServer.State == 0
                ? Packet.UnavailableServer
                : string.Format("{0}{1}:{2};{3}", Packet.ConnectToGameServer, gameServer.Ip, gameServer.Port,
                    Hash.RandomString(20)));
        }

        public enum StateConnecion
        {
            CheckingVersion,
            CheckingAccount,
            CheckingServer,
        }
    }
}
