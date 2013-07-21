using System;
using SilverSock;

namespace SilverRealm.Network.Abstract
{
    abstract class Server
    {
        protected SilverServer Serv;

        protected Server(string ip, int port)
        {
            Serv = new SilverServer(ip, port);
            {
                Serv.OnAcceptSocketEvent += OnSocketAccepted;
                Serv.OnListeningEvent += OnListening;
                Serv.OnListeningFailedEvent += OnListeningFailed;
                Serv.WaitConnection();
            }
        }

        #region abstracts

        public abstract void OnSocketAccepted(SilverSocket socket);
        public abstract void OnListening();
        public abstract void OnListeningFailed(Exception e);

        #endregion
    }
}
