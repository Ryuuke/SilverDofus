using System;
using System.Linq;
using System.Text;
using SilverSock;

namespace SilverGame.Network.Abstract
{
    abstract class Client
    {
        protected SilverSocket Socket;

        public Client(SilverSocket socket)
        {
            this.Socket = socket;
            {
                socket.OnConnected += this.OnConnected;
                socket.OnDataArrivalEvent += this.DataArrival;
                socket.OnFailedToConnect += this.OnFailedToConnect;
                socket.OnSocketClosedEvent += this.OnSocketClosed;
            }
        }

        #region abstracts

        public abstract void OnConnected();
        public abstract void OnFailedToConnect(Exception e);
        public abstract void OnSocketClosed();
        public abstract void DataReceived(string packet);

        #endregion

        public virtual void SendPackets(string packet)
        {
            Console.WriteLine("send >>" + string.Format("{0}\x00", packet));
            this.Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", packet)));
        }

        public void DataArrival(byte[] data)
        {
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                Console.WriteLine("Recv <<" + packet);
                DataReceived(packet);
            }
        }
    }
}
