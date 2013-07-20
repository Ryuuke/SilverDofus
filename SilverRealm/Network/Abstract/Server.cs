using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverRealm.Network.Abstract
{
    abstract class Server
    {
        protected SilverServer serv;

        public Server(string ip, int port)
        {
            serv = new SilverServer(ip, port);
            {
                serv.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(onSocketAccepted);
                serv.OnListeningEvent += new SilverEvents.Listening(onListening);
                serv.OnListeningFailedEvent += new SilverEvents.ListeningFailed(onListeningFailed);
                serv.WaitConnection();
            }
        }

        #region abstracts

        public abstract void onSocketAccepted(SilverSocket socket);
        public abstract void onListening();
        public abstract void onListeningFailed(Exception e);

        #endregion
    }
}
