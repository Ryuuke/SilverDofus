using System;
using SilverSock;

namespace SilverGame.Network.Abstract
{
    abstract class Server
    {
        private readonly SilverServer _serv;

        protected Server(string ip, int port)
        {
            _serv = new SilverServer(ip, port);
            {
                _serv.OnAcceptSocketEvent += OnSocketAccepted;
                _serv.OnListeningEvent += OnListening;
                _serv.OnListeningFailedEvent += OnListeningFailed;
            }
        }

        public void Run()
        {
            _serv.WaitConnection();
        }

        #region abstracts

        protected abstract void OnSocketAccepted(SilverSocket socket);
        protected abstract void OnListening();
        protected abstract void OnListeningFailed(Exception e);

        #endregion
    }
}
