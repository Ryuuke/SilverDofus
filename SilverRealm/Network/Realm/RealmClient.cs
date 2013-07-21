using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverRealm.Network.Realm
{
    class RealmClient : Abstract.Client
    {
        private string _key;

        private StateConnecion stateConnxion;
        private Models.Account account;

        private List<Models.GameServer> gameServers;

        public RealmClient(SilverSocket socket)
            : base(socket)
        {
            _key = Services.Hash.RandomString(32);

            sendPackets(string.Format("{0}{1}", Services.Packet.HelloConnectionServer, _key));
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

            lock (RealmServer._lock)
                RealmServer.Clients.Remove(this);
        }

        public override void dataReceived(string packet)
        {
            switch (stateConnxion)
            {
                case StateConnecion.CheckingVersion:
                    stateConnxion = StateConnecion.CheckingAccount;
                    checkVersion(packet);
                    break;

                case StateConnecion.CheckingAccount:
                    stateConnxion = StateConnecion.CheckingServer;
                    checkAccount(packet);
                    break;

                case StateConnecion.CheckingServer:
                    CheckQueue(packet);
                    break;
            }
        }

        public override void sendPackets(string packet)
        {
            base.sendPackets(packet);
        }

        public void checkVersion(string packet)
        {
            if (Services.Constant.Version != packet)
            {
                this.sendPackets(Services.Packet.WrongDofusVersion);
                this.OnSocketClosed();
            }
        }

        public void checkAccount(string packet)
        {
            string username = packet.Split('#')[0];
            string password = packet.Split('#')[1];
            
            account = Database.AccountRepository.getAccount(username);

            if (account == null || Services.Hash.Encrypt(account.password, _key) != password)
            {
                this.sendPackets(Services.Packet.WrongDofusAccount);
                this.OnSocketClosed();
            }
            else if (RealmServer.Clients.Count(x => x.account.username == username) > 1)
            {
                this.sendPackets(Services.Packet.AlredyConnected);
                this.OnSocketClosed();
            }
            else if (account.bannedUntil != null && account.bannedUntil > DateTime.Now)
            {
                this.sendPackets(Services.Packet.bannedAccount);
                this.OnSocketClosed();
            }
            else
            {
                this.sendPackets(string.Format("{0}{1}", Services.Packet.DofusPseudo, account.pseudo));
                this.sendPackets(string.Format("{0}{1}", Services.Packet.Community, 0)); // 0 : communauté fr

                sendServers();

                this.sendPackets(string.Format("{0}{1}", Services.Packet.IsAdmin, account.gmLevel > 0 ? 1 : 0));
                this.sendPackets(string.Format("{0}{1}", Services.Packet.SecretQuestion, account.question.Replace(" ", "+")));
            }
        }

        private void sendServers()
        {
            gameServers = Database.GameServerRepository.getAll();

            this.sendPackets(string.Format("{0}{1}", Services.Packet.Hosts, string.Join("|", gameServers)));
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
                    sendServersList();
                    break;
            }
        }

        private void CheckQueue()
        {
            this.sendPackets(string.Format("{0}{1}", Services.Packet.NewQueue, "|0|0|1|-1"));
        }

        private void sendServersList()
        {
            Dictionary<int, int> ListCharactersByGameServer = Database.Characters.getCharactersByGameServer(account.id);

            string dicToString = string.Empty;

            foreach (var item in ListCharactersByGameServer)
            {
                dicToString = string.Concat(dicToString, string.Format("|{0},{1}", item.Key, item.Value));
            }

            if (bool.Parse(Services.Config.get("subscription")) == true)
            {
                if (account.subscription == null || DateTime.Now >= account.subscription.Value)
                {
                    this.sendPackets(string.Format("{0}{1}{2}", Services.Packet.SubscriptionPlayerList, Services.Constant.DiscoveryMode, dicToString));
                }
                else
                {
                    this.sendPackets(string.Format("{0}{1}{2}", Services.Packet.SubscriptionPlayerList, (account.subscription.Value - DateTime.Now).TotalMilliseconds.ToString().Split(',')[0], dicToString));
                }
            }
            else
            {
                this.sendPackets(string.Format("{0}{1}{2}", Services.Packet.SubscriptionPlayerList, Services.Constant.OneYear, dicToString));
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
