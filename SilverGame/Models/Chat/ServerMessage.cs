using System.Linq;
using SilverGame.Database;
using SilverGame.Models.Characters;
using SilverGame.Network.Game;
using SilverGame.Services;

namespace SilverGame.Models.Chat
{
    class ServerMessage
    {
        private readonly Character _character;

        public ServerMessage(Character character)
        {
            _character = character;
        }

        public void SendDefaultMessage(string message)
        {
            _character.Map.Send(string.Format("{0}|{1}|{2}|{3}", Packet.DefaultMessage, _character.Id, _character.Name, message));
        }

        public void SendRecruitmentMessage(string message)
        {
            _character.SendToAll(string.Format("{0}|{1}|{2}|{3}", Packet.RecruitmentMessage, _character.Id, _character.Name, message));
        }

        public void SendBusinessMessage(string message)
        {
            _character.SendToAll(string.Format("{0}|{1}|{2}|{3}", Packet.BusinessMessage, _character.Id, _character.Name, message));
        }

        public void SendPrivateMessage(string receiverName, string message)
        {
            var clientSender = GameServer.Clients.Find(x => x.Character == _character);

            var receiver = DatabaseProvider.Characters.Find(x => x.Name.Equals(receiverName));

            if (receiver == null)
            {
                clientSender.SendPackets(string.Format("{0}{1}", Packet.PrivateMessageNotConnectedReceiverError, receiverName));
                return;
            }

            var clientReceiver = GameServer.Clients.Find(x => x.Character == receiver);

            clientReceiver.SendPackets(string.Format("{0}|{1}|{2}|{3}", Packet.PrivateMessageReceiver, _character.Id, _character.Name, message));
            clientSender.SendPackets(string.Format("{0}|{1}|{2}|{3}", Packet.PrivateMessageSender, _character.Id, _character.Name, message));
        }

        public void SendAdminMessage(string message)
        {
            var account = DatabaseProvider.AccountCharacters.Find(x => x.Character == _character).Account;

            if (account.GmLevel == 0)
                return;

            lock (GameServer.Lock)
            {
                foreach (var gameClient in GameServer.Clients.Where(x => x.Account.GmLevel > 0))
                {
                    gameClient.SendPackets(string.Format("{0}|{1}|{2}|{3}", Packet.AdminMesage, _character.Id, _character.Name, message));
                } 
            }
        }
    }
}