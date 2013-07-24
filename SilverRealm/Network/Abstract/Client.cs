using System;
using System.Linq;
using System.Text;
using SilverRealm.Services;
using SilverSock;

namespace SilverRealm.Network.Abstract
{
    abstract class Client
    {
        protected SilverSocket Socket;

        protected Client(SilverSocket socket)
        {
            Socket = socket;
            {
                socket.OnConnected += OnConnected;
                socket.OnDataArrivalEvent += DataArrival;
                socket.OnFailedToConnect += OnFailedToConnect;
                socket.OnSocketClosedEvent += OnSocketClosed;
            }
        }

        #region abstracts

        protected abstract void OnConnected();
        protected abstract void OnFailedToConnect(Exception e);
        protected abstract void OnSocketClosed();
        protected abstract void DataReceived(string packet);

        #endregion

        protected virtual void SendPackets(string packet)
        {
            SilverConsole.WriteLine(string.Format("send >>" + string.Format("{0}\x00", packet)), ConsoleColor.Cyan);
            Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\x00", packet)));
        }

        protected void DataArrival(byte[] data)
        {
            foreach (var packet in Encoding.UTF8.GetString(data).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                SilverConsole.WriteLine("Recv <<" + packet, ConsoleColor.Green);
                DataReceived(packet);
            }
        }
    }
}
