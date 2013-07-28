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
            }
        }

        public void Run()
        {
            Serv.WaitConnection();
        }

        #region abstracts

        protected abstract void OnSocketAccepted(SilverSocket socket);
        protected abstract void OnListening();
        protected abstract void OnListeningFailed(Exception e);

        #endregion
    }
}
