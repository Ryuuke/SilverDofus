using System;
using System.Collections.Generic;
using System.Linq;
using SilverSock;

namespace SilverRealm.Network.Realm
{
    sealed class RealmClient : Abstract.Client
    {
        private readonly string _key;

        private StateConnecion _stateConnxion;
        private Models.Account _account;

        private List<Models.GameServer> _gameServers;

        public RealmClient(SilverSocket socket)
            : base(socket)
        {
            _key = Services.Hash.RandomString(32);

            SendPackets(string.Format("{0}{1}", Services.Packet.HelloConnectionServer, _key));
        }

        public override void OnConnected()
        {

        }

        public override void OnFailedToConnect(Exception e)
        {
            Console.WriteLine("Failed to connect to client");
        }

        public override void OnSocketClosed()
        {
            Console.WriteLine("Connection closed");

            lock (RealmServer.Lock)
                RealmServer.Clients.Remove(this);
        }

        public override void DataReceived(string packet)
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

        public void CheckVersion(string packet)
        {
            if (Services.Constant.Version == packet) return;
                this.SendPackets(Services.Packet.WrongDofusVersion);
                this.OnSocketClosed();
        }

        public void CheckAccount(string packet)
        {
            var username = packet.Split('#')[0];
            var password = packet.Split('#')[1];
            
            _account = Database.AccountRepository.GetAccount(username);

            if (_account == null || Services.Hash.Encrypt(_account.Password, _key) != password)
            {
                this.SendPackets(Services.Packet.WrongDofusAccount);
                this.OnSocketClosed();
            }
            else if (RealmServer.Clients.Count(x => x._account.Username == username) > 1)
            {
                this.SendPackets(Services.Packet.AlredyConnected);
                this.OnSocketClosed();
            }
            else if (_account.BannedUntil != null && _account.BannedUntil > DateTime.Now)
            {
                this.SendPackets(Services.Packet.BannedAccount);
                this.OnSocketClosed();
            }
            else
            {
                this.SendPackets(string.Format("{0}{1}", Services.Packet.DofusPseudo, _account.Pseudo));
                this.SendPackets(string.Format("{0}{1}", Services.Packet.Community, 0)); // 0 : communauté fr

                SendServers();

                this.SendPackets(string.Format("{0}{1}", Services.Packet.IsAdmin, _account.GmLevel > 0 ? 1 : 0));
                this.SendPackets(string.Format("{0}{1}", Services.Packet.SecretQuestion, _account.Question.Replace(" ", "+")));
            }
        }

        private void SendServers()
        {
            _gameServers = Database.GameServerRepository.GetAll();

            this.SendPackets(string.Format("{0}{1}", Services.Packet.Hosts, string.Join("|", _gameServers)));
        }

        private void CheckQueue(string packet)
        {
            if (!packet.StartsWith("A"))
                return;

            switch (packet)
            {
                case Services.Packet.NewQueue:
                    CheckQueue();
                    break;

                case Services.Packet.ServersList:
                    SendServersList();
                    break;
            }
        }

        private void CheckQueue()
        {
            this.SendPackets(string.Format("{0}{1}", Services.Packet.NewQueue, "|0|0|1|-1"));
        }

        public void SendServersList()
        {
            var listCharactersByGameServer = Database.Characters.GetCharactersByGameServer(_account.Id);

            if (bool.Parse(Services.Config.Get("subscription")) == true)
            {
                if (_account.Subscription == null || DateTime.Now >= _account.Subscription.Value)
                {
                    this.SendPackets(string.Format("{0}{1}{2}", Services.Packet.SubscriptionPlayerList, Services.Constant.DiscoveryMode, listCharactersByGameServer));
                }
                else
                {
                    this.SendPackets(string.Format("{0}{1}{2}", Services.Packet.SubscriptionPlayerList, (_account.Subscription.Value - DateTime.Now).TotalMilliseconds.ToString().Split(',')[0], listCharactersByGameServer));
                }
            }
            else
            {
                this.SendPackets(string.Format("{0}{1}{2}", Services.Packet.SubscriptionPlayerList, Services.Constant.OneYear, listCharactersByGameServer));
            }
        }

        public enum StateConnecion
        {
            CheckingVersion,
            CheckingAccount,
            CheckingServer,
        }
    }
}
