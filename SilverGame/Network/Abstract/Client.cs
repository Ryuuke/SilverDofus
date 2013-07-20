using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverSock;

namespace SilverGame.Network.Abstract
{
    abstract class Client
    {
        protected SilverSocket socket;

        public Client(SilverSocket socket)
        {
            this.socket = socket;
            {
                socket.OnConnected += new SilverEvents.Connected(this.OnConnected);
                socket.OnDataArrivalEvent += new SilverEvents.DataArrival(this.dataArrival);
                socket.OnFailedToConnect += new SilverEvents.FailedToConnect(this.OnFailedToConnect);
                socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(this.OnSocketClosed);
            }
        }

        #region abstracts

        public abstract void OnConnected();
        public abstract void OnFailedToConnect(Exception e);
        public abstract void OnSocketClosed();
        public abstract void dataReceived(string packet);

        #endregion

        public virtual void sendPackets(string packet)
        {
            Console.WriteLine("send >>" + string.Format("{0}\x00", packet));
            this.socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", packet)));
        }

        public void dataArrival(byte[] data)
        {
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                Console.WriteLine("Recv <<" + packet);
                dataReceived(packet);
            }
        }
    }
}
