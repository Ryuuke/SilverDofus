using System;
using System.Threading;
using SilverSock;

namespace SilverGame.Network.ToRealm
{
    sealed class ToRealmClient : Abstract.Client
    {
        public ToRealmClient()
            : base(new SilverSocket())
        {
            ConnectToRealm();
        }

        public void ConnectToRealm()
        {
            Socket.ConnectTo(Services.Config.Get("Realm_ip"), Int32.Parse(Services.Config.Get("Com_port")));
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to Realm Server Sucessfuly");
            
            Thread.Sleep(500);

            SendPackets(string.Format("{0}{1}", Services.Packet.HelloRealm, Services.Config.Get("Game_key")));
        }

        protected override void OnFailedToConnect(Exception e)
        {
            Console.WriteLine("Failed to connect to Realm Server");

            RetryToConnect();
        }

        protected override void OnSocketClosed()
        {
            Console.WriteLine("Connexion To Realm Server closed");

            RetryToConnect();
        }

        private void RetryToConnect()
        {
            Thread.Sleep(10000);

            ConnectToRealm();
        }

        protected override void DataReceived(string packet)
        {
            throw new NotImplementedException();
        }
    }
}
