using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SilverGame.Database;
using SilverGame.Database.Repository;
using SilverGame.Models.Accounts;
using SilverGame.Services;

namespace SilverGame.Network.Game.GameParser
{
    internal class GameParser
    {
        private readonly GameClient _client;

        private delegate void Response(string data);

        private readonly Dictionary<string, Response> _packetRegistry;

        public GameParser(GameClient client)
        {
            _client = client;

            _client.SendPackets(Packet.HelloGameServer);

            _packetRegistry = new Dictionary<string, Response>();

            InitRegistry();
        }

        private void InitRegistry()
        {
            _packetRegistry.Add(Packet.TicketResponse, ParseTicket);
            _packetRegistry.Add(Packet.RegionalVersion, SendRegionalVersion);
            _packetRegistry.Add(Packet.CharactersList, SendCharactersList);
            _packetRegistry.Add(Packet.CharacterNameGenerated, GenerateName);
            _packetRegistry.Add(Packet.CharacterAdd, AddCharacter);
            _packetRegistry.Add(Packet.GiftsList, SendGiftsList);
            _packetRegistry.Add(Packet.GiftStored, AddGiftItem);
            _packetRegistry.Add(Packet.CharacterSelected, SendCharacterInfos);
            _packetRegistry.Add(Packet.GameCreated, SendCharacterName);
        }

        public void Parse(string packet)
        {
            var header = packet.Substring(0, 2);

            if (!_packetRegistry.ContainsKey(header))
                return;

            var response = _packetRegistry[header];

            response(packet.Substring(2));
        }

        // Respones functions

        private void ParseTicket(string packet)
        {
            var date = packet.Split('|')[0];

            var ip = packet.Split('|')[1];

            if (DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", null).AddSeconds(Constant.TicketTimeExpiredInterval) <
                DateTime.ParseExact(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), "MM/dd/yyyy HH:mm:ss", null))
            {
                _client.SendPackets(Packet.TicketExpired);
                _client.OnSocketClosed();
            }
            else if (ip.Equals(_client.Socket.IP))
            {
                _client.SendPackets(Packet.InvalidIp);
                _client.OnSocketClosed();
            }
            else
            {
                var account = packet.Split('|')[2].Split(',');

                try
                {
                    _client.Account = new Account
                    {
                        Id = int.Parse(account[0]),
                        Username = account[1],
                        Password = account[2],
                        Pseudo = account[3],
                        Question = account[4],
                        Reponse = account[5],
                        Connected = true,
                        GmLevel = int.Parse(account[7]),
                        BannedUntil =
                            account[8] == ""
                                ? (DateTime?) null
                                : DateTime.Parse(account[8].ToString(CultureInfo.InvariantCulture)),
                        Subscription =
                            account[9] == ""
                                ? (DateTime?) null
                                : DateTime.Parse(account[9].ToString(CultureInfo.InvariantCulture)),
                    };

                    AccountRepository.UpdateAccount(true, _client.Account.Id);
                }
                catch (Exception e)
                {
                    SilverConsole.WriteLine(e.Message, ConsoleColor.Red);
                    Logs.LogWritter(Constant.ErrorsFolder,
                        string.Format("Impossible de charger le compte {0}", account[1]));
                }

                _client.SendPackets(Packet.TicketAccepted);
            }
        }

        private void SendRegionalVersion(string data)
        {
            _client.SendPackets(string.Format("{0}{1}", Packet.RegionalVersion, Config.Get("Community")));
        }

        private void SendCharactersList(string data)
        {
            var characters =
                DatabaseProvider.AccountCharacters.Where(
                    accountCharacters => accountCharacters.Account.Id == _client.Account.Id)
                    .Aggregate(string.Empty,
                        (current, accountCharacters) =>
                            current + accountCharacters.Character.InfosWheneChooseCharacter());

            if (bool.Parse(Config.Get("Subscription")))
            {
                _client.SendPackets(string.Format("{0}{1}|{2}{3}", Packet.CharactersListResponse,
                    _client.Account.Subscription == null || _client.Account.Subscription.Value < DateTime.Now
                        ? (object) Constant.DiscoveryMode
                        : (_client.Account.Subscription.Value - DateTime.Now).TotalMilliseconds.ToString(
                            CultureInfo.InvariantCulture).Split('.')[0],
                    DatabaseProvider.AccountCharacters.Count(x => x.Account.Id == _client.Account.Id),
                    characters));
            }
            else
            {
                _client.SendPackets(string.Format("{0}{1}|{2}{3}", Packet.CharactersListResponse,
                    Constant.OneYear,
                    DatabaseProvider.AccountCharacters.Count(x => x.Account.Id == _client.Account.Id),
                    characters));
            }
        }

        private void GenerateName(string data)
        {
            _client.SendPackets(string.Format("{0}{1}", Packet.CharacterNameGeneratedResponse,
                Algorithm.GenerateRandomName()));
        }

        private void AddCharacter(string data)
        {
            var datas = data.Split('|');

            var name = datas[0];

            var classe = int.Parse(datas[1]);

            var sex = int.Parse(datas[2]);

            var color1 = int.Parse(datas[3]);

            var color2 = int.Parse(datas[4]);

            var color3 = int.Parse(datas[5]);

            if (DatabaseProvider.Characters.All(x => x.Name != name) && name.Length >= 3 && name.Length <= 20)
            {
                var reg = new Regex("^[a-zA-Z-]+$");

                if (reg.IsMatch(name) && name.Count(c => c == '-') < 3)
                {
                    if (classe >= 1 && classe <= 12 && (sex == 1 || sex == 0))
                    {
                        CharacterRepository.Create(name, classe, sex, color1, color2, color3, _client.Account.Id);
                        _client.SendPackets(Packet.CreationSuccess);
                        SendCharactersList("");
                    }
                    else
                    {
                        _client.SendPackets(Packet.CreateCharacterFullErrors);
                    }
                }
                else
                {
                    _client.SendPackets(Packet.CreateCharacterBadName);
                }
            }
            else
            {
                _client.SendPackets(Packet.NameAlredyExists);
            }
        }

        private void SendGiftsList(string data)
        {
            var gift = DatabaseProvider.AccountGifts.Find(x => x.Key.Id == _client.Account.Id).Value;

            if (gift == null)
                return;

            _client.SendPackets(string.Format("{0}1|{1}", Packet.GiftsList, gift));
        }

        private void AddGiftItem(string data)
        {
            var gifts = DatabaseProvider.ItemGift.FindAll(x => x.GiftId == int.Parse(data.Split('|')[0]));
            var character = DatabaseProvider.Characters.Find(x => x.Id == int.Parse(data.Split('|')[1]));
            
            foreach (var gift in gifts)
            {
                for (var i = 0; i < gift.Quantity; i++)
                {
                   gift.Item.Generate(character, gift.Quantity);
                }
            }

            GiftRepository.RemoveFromAccount(gifts.First().GiftId, _client.Account.Id);

            _client.SendPackets(Packet.GiftStotedSuccess);
        }

        private void SendCharacterInfos(string data)
        {
            var character = DatabaseProvider.Characters.Find(x => x.Id == int.Parse(data));

            if (character == null)
                return;

            _client.SendPackets(string.Format("{0}|{1}", Packet.CharacterSelectedResponse, character.InfosWheneSelectedCharacter()));
        }

        private void SendCharacterName(string data)
        {
            var character = DatabaseProvider.AccountCharacters.FindAll(x => x.Account.Id == _client.Account.Id).ElementAt(int.Parse(data) -1).Character;

            if (character == null)
                return;

                _client.SendPackets(string.Format("{0}|1|{1}", Packet.GameCreatedResponse, character.Name));
            try
            {
                _client.SendPackets(string.Format("{0}{1}", Packet.Stats, character.GetStats()));
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(e.Message,ConsoleColor.Red);
            }
        }
    }
}