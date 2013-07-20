using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverGame.Network.ToRealm
{
    class ToRealmClient : Abstract.Client
    {
        public ToRealmClient()
            : base(new SilverSocket())
        {
            connectToRealm();

            this.sendPackets(string.Format("{0}", Services.Packet.HelloRealm));
        }

        public void connectToRealm()
        {
            socket.ConnectTo(Services.Config.get("Realm_ip"), Int32.Parse(Services.Config.get("Com_port")));
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

        public override void sendPackets(string packet)
        {
            base.sendPackets(packet);
        }

        public override void dataReceived(string packet)
        {
            throw new NotImplementedException();
        }
    }
}
