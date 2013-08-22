using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SilverGame.Database;
using SilverGame.Database.Repository;
using SilverGame.Models.Accounts;
using SilverGame.Models.Characters;
using SilverGame.Models.Chat;
using SilverGame.Models.Items;
using SilverGame.Models.Maps;
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
            _packetRegistry.Add(Packet.GameCreated, SendCharacterGameInfos);
            _packetRegistry.Add(Packet.GameInfos, SendGameInfos);
            _packetRegistry.Add(Packet.GameActions, ParseGameAction);
            _packetRegistry.Add(Packet.GameEndAction, ParseEndGameAction);
            _packetRegistry.Add(Packet.ObjectMove, MoveItem);
            _packetRegistry.Add(Packet.ObjectRemove, RemoveItem);
            _packetRegistry.Add(Packet.SubscribeChannel, SubscribeChannel);
            _packetRegistry.Add(Packet.ServerMessage, SendMessage);
            _packetRegistry.Add(Packet.StatsBoost, StatsBoost);
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
            var charactersNumber = DatabaseProvider.AccountCharacters.FindAll(x => x.Account.Id == _client.Account.Id).Count;

            if (charactersNumber >= int.Parse(Config.Get("Player_Max_Per_Account")))
            {
                _client.SendPackets(Packet.CreateCharacterFullErrors);
                return;
            }

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
                        var newCharacter = new Character
                        {
                            Id =
                                DatabaseProvider.Characters.Count > 0
                                    ? DatabaseProvider.Characters.OrderByDescending(x => x.Id).First().Id + 1
                                    : 1,
                            Name = name,
                            Classe = (Character.Class) classe,
                            Sex = sex,
                            Color1 = color1,
                            Color2 = color2,
                            Color3 = color3,
                            Level = int.Parse(Config.Get("Starting_level")),
                            Skin = int.Parse(classe + "" + sex),
                            PdvNow =
                                (int.Parse(Config.Get("Starting_level")) - 1)*Character.GainHpPerLvl + Character.BaseHp,
                            PdvMax =
                                (int.Parse(Config.Get("Starting_level")) - 1)*Character.GainHpPerLvl + Character.BaseHp,
                        };

                        newCharacter.GenerateInfos(_client.Account.GmLevel);

                        CharacterRepository.Create(newCharacter, _client.Account.Id);

                        _client.SendPackets(Packet.CreationSuccess);

                        _client.SendPackets(Packet.GameBegin);

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
            var gifts = DatabaseProvider.ItemGift.FindAll(
                x => x.GiftId == int.Parse(data.Split('|')[0]));

            var character = DatabaseProvider.Characters.Find(
                x => x.Id == int.Parse(data.Split('|')[1]));

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

            _client.Character = character;
            _client.SendPackets(string.Format("{0}|{1}", 
                Packet.CharacterSelectedResponse,
                character.InfosWheneSelectedCharacter()));
        }

        private void SendCharacterGameInfos(string data)
        {
            if (_client.Character == null)
                return;

            _client.SendPackets(string.Format("{0}|1|{1}", Packet.GameCreatedResponse, _client.Character.Name));

            _client.SendPackets(string.Format("{0}{1}", Packet.Stats, _client.Character.GetStats()));

            _client.SendPackets(Packet.Restriction);

            _client.SendPackets(string.Format("{0}{1}", Packet.Message, Packet.WelcomeToDofus));

            _client.SendPackets(string.Format("{0}+{1}", Packet.SubscribeChannel,
                string.Join("", _client.Character.Channels)));

            _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectWeight,
                _client.Character.GetCurrentWeight(), _client.Character.GetMaxWeight()));

            LoadMap();
        }

        private void LoadMap()
        {
            _client.SendPackets(string.Format("{0}|{1}|{2}|{3}", Packet.MapData, _client.Character.Map.Id,
                _client.Character.Map.Time, _client.Character.Map.Key));
        }

        private void SendGameInfos(string data)
        {
            _client.Character.Map.AddCharacter(_client.Character);

            _client.Character.Map.Send(string.Format("{0}{1}", Packet.Movement, _client.Character.Map.DisplayChars()));

            _client.SendPackets(Packet.MapLoaded);
        }

        private void ParseGameAction(string data)
        {
            switch (data.Substring(0, 1))
            {
                case "0":
                    Move(data);
                    break;
            }
        }

        private void Move(string data)
        {
            data = data.Substring(3);

            var path = new PathFinding(
                data,
                _client.Character.Map,
                _client.Character.MapCell,
                _client.Character.Direction);

            var newPath = path.RemakePath();

            newPath = path.GetStartPath + newPath;

            if (!_client.Character.Map.Cells.Contains(path.Destination))
            {
                _client.SendPackets(string.Format("{0};0", Packet.GameActions));
                return;
            }

            _client.Character.Direction = path.Direction;
            _client.Character.CellDestination = path.Destination;

            _client.Character.Map.Send(string.Format("{0}0;1;{1};{2}", Packet.GameActions, _client.Character.Id, newPath));

        }

        private void ParseEndGameAction(string data)
        {
            switch (data.Substring(0, 1))
            {
                case "E":
                    ChangeDestination(data);
                    break;

                case "K":
                    EndMove();
                    break;
            }
        }

        private void ChangeDestination(string data)
        {
            var cell = int.Parse(data.Split('|')[1]);

            if (_client.Character.Map.Cells.Contains(cell))
                _client.Character.MapCell = cell;
        }

        private void EndMove()
        {
            _client.Character.MapCell = _client.Character.CellDestination;

            var trigger =
                DatabaseProvider.MapTriggers.Find(
                    x => x.Cell == _client.Character.MapCell
                         && x.Map == _client.Character.Map);

            if (trigger != null)
                TeleportPlayer(trigger.NewMap, trigger.NewCell);
            else
                _client.SendPackets(Packet.Nothing);
        }

        private void TeleportPlayer(Map newMap, int newCell)
        {
            _client.Character.Map.RemoveCharacter(_client.Character);

            _client.SendPackets(string.Format("{0};2;{1};", Packet.GameActions, _client.Character.Id));

            _client.Character.MapCell = newCell;
            _client.Character.Map = newMap;

            LoadMap();
        }

        // TODO : verify item conditions

        private void MoveItem(string data)
        {
            var itemId = int.Parse(data.Split('|')[0]);

            var itemPosition = (StatsManager.Position) int.Parse(data.Split('|')[1]);

            var item =
                DatabaseProvider.InventoryItems.Find(
                    x => 
                        x.Id == itemId && 
                        x.Character.Id == _client.Character.Id);

            if (item == null)
                return;

            if (itemPosition != StatsManager.Position.None && item.ItemInfos.Level > _client.Character.Level)
                return;

            if (itemPosition == StatsManager.Position.None)
            {
                var existItem = DatabaseProvider.InventoryItems.Find(
                    x =>
                        x.ItemInfos == item.ItemInfos &&
                        string.Join(",", x.Stats).Equals(string.Join(",", item.Stats)) &&
                        x.Character == item.Character && x.ItemPosition == itemPosition);

                if (existItem != null)
                {
                    var id = item.Id;

                    InventoryItemRepository.Remove(item);

                    _client.SendPackets(string.Format("{0}{1}", Packet.ObjectRemove, id));

                    existItem.Quantity += 1;

                    InventoryItemRepository.Update(existItem);

                    _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, existItem.Id,
                        existItem.Quantity));
                }
                else
                {
                    item.ItemPosition = StatsManager.Position.None;
                    InventoryItemRepository.Update(item);

                    _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectMove, item.Id, (int)itemPosition));
                }

                _client.Character.Stats.RemoveItemStats(item.Stats);
            }
            else
            {
                var existItem = DatabaseProvider.InventoryItems.Find(
                    x =>
                        x.Character == _client.Character &&
                        x.ItemPosition == itemPosition);

                // TODO : debug Pets food & Obvis

                if (existItem != null)
                {
                    _client.SendPackets(Packet.Nothing);
                    return;
                }

                if (item.Quantity > 1)
                {
                    var newItem = item.Copy(quantity: item.Quantity - 1);

                    InventoryItemRepository.Create(newItem);

                    item.Quantity = 1;

                    item.ItemPosition = itemPosition;

                    InventoryItemRepository.Update(item);

                    _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectMove, item.Id, (int) itemPosition));

                    _client.SendPackets(string.Format("{0}{1}", Packet.ObjectAdd, newItem.ItemInfo()));
                }
                else
                {
                    item.ItemPosition = itemPosition;

                    InventoryItemRepository.Update(item);

                    _client.SendPackets(string.Format("{0}{1}", Packet.ObjectMove, data));
                }

                _client.Character.Stats.AddItemStats(item.Stats);
            }

            _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectWeight,
                    _client.Character.GetCurrentWeight(), _client.Character.GetMaxWeight()));

            _client.SendPackets(string.Format("{0}{1}", Packet.Stats, _client.Character.GetStats()));

            _client.Character.Map.Send(string.Format("{0}{1}|{2}", Packet.ObjectAccessories, _client.Character.Id,
                _client.Character.GetItemsWheneChooseCharacter()));
        }

        private void RemoveItem(string data)
        {
            throw new NotImplementedException();
        }

        private void SubscribeChannel(string data)
        {
            if (!Channel.Headers.Contains(data[1]))
                return;

            var state = data[0];
            var header = (Channel.ChannelHeader) data[1];

            if (state.Equals('+'))
            {
                if (_client.Character.Channels.All(x => x.Header != header))
                {
                    _client.Character.Channels.Add(new Channel
                    {
                        Header = header
                    });
                }
            }
            else
            {
                if (_client.Character.Channels.Any(x => x.Header == header))
                {
                    _client.Character.Channels.RemoveAll(x => x.Header == header);
                }
            }

            _client.SendPackets(string.Format("{0}{1}", Packet.SubscribeChannel, data));
        }

        private void SendMessage(string data)
        {
            var channelHeader = (Channel.ChannelHeader) data.Split('|')[0][0];

            var receiver = data.Split('|')[0];
            var message = data.Split('|')[1];

            var serverMessage = new ServerMessage(_client.Character);

            switch (channelHeader)
            {
                case Channel.ChannelHeader.DefaultChannel:
                    serverMessage.SendDefaultMessage(message);
                    break;
                case Channel.ChannelHeader.RecruitmentChannel:
                    serverMessage.SendRecruitmentMessage(message);
                    break;
                case Channel.ChannelHeader.BusinessChannel:
                    serverMessage.SendBusinessMessage(message);
                    break;
                case Channel.ChannelHeader.AdminChannel:
                    serverMessage.SendAdminMessage(message);
                    break;
                case Channel.ChannelHeader.UndefinedChannel:
                    break;
                default:
                    serverMessage.SendPrivateMessage(receiver, message);
                    break;
            }
        }

        private void StatsBoost(string data)
        {
            try
            {
                var baseStats = int.Parse(data);

                if (baseStats < 10 || baseStats > 15)
                    return;

                _client.Character.BoostStats((Character.BaseStats)baseStats);

                _client.SendPackets(string.Format("{0}{1}", Packet.Stats, _client.Character.GetStats()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

    }
}