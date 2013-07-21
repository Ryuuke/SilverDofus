using System;
using SilverSock;

namespace SilverGame.Network.ToRealm
{
    class ToRealmClient : Abstract.Client
    {
        public ToRealmClient()
            : base(new SilverSocket())
        {
            ConnectToRealm();

            this.SendPackets(string.Format("{0}", Services.Packet.HelloRealm));
        }

        public void ConnectToRealm()
        {
            Socket.ConnectTo(Services.Config.get("Realm_ip"), Int32.Parse(Services.Config.get("Com_port")));
        }

        public override void OnConnected()
        {
            Console.WriteLine("Connected to Realm Server Sucessfuly");
        }

        public override void OnFailedToConnect(Exception e)
        {
            Console.WriteLine("Failed to connect to Realm Server");
        }

        public override void OnSocketClosed()
        {
            Console.WriteLine("Connexion To Realm Server closed");
        }

        public override sealed void SendPackets(string packet)
        {
            base.SendPackets(packet);
        }

        public override void DataReceived(string packet)
        {
            throw new NotImplementedException();
        }
    }
}
