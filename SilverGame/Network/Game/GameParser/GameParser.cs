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
using SilverGame.Models.Exchange;
using SilverGame.Models.Items.Items;
using SilverGame.Models.Items.ItemSets;
using SilverGame.Models.Maps;
using SilverGame.Services;

namespace SilverGame.Network.Game.GameParser
{
    internal class GameParser
    {
        private readonly GameClient _client;

        private readonly Dictionary<string, Action<string>> _charactersPacketRegistry;
        private readonly Dictionary<string, Action<string>> _inGamePacketRegistry;

        private PacketState _packetState;

        public GameParser(GameClient client)
        {
            _client = client;

            _charactersPacketRegistry = new Dictionary<string, Action<string>>();
            _inGamePacketRegistry = new Dictionary<string, Action<string>>();

            InitCharactersRegistry();
            InitInGameRegistry();

            _client.SendPackets(Packet.HelloGame);
        }

        private void InitCharactersRegistry()
        {
            _charactersPacketRegistry.Add(Packet.RegionalVersion, SendRegionalVersion);
            _charactersPacketRegistry.Add(Packet.CharactersList, SendCharactersList);
            _charactersPacketRegistry.Add(Packet.CharacterNameGenerated, GenerateName);
            _charactersPacketRegistry.Add(Packet.CharacterAdd, AddCharacter);
            _charactersPacketRegistry.Add(Packet.GiftsList, SendGiftsList);
            _charactersPacketRegistry.Add(Packet.GiftStored, AddGiftItem);
            _charactersPacketRegistry.Add(Packet.CharacterSelected, SendCharacterInfos);
        }

        private void InitInGameRegistry()
        {
            _inGamePacketRegistry.Add(Packet.GameInfos, SendGameInfos);
            _inGamePacketRegistry.Add(Packet.GameActions, ParseGameAction);
            _inGamePacketRegistry.Add(Packet.GameEndAction, ParseEndGameAction);
            _inGamePacketRegistry.Add(Packet.ObjectMove, MoveItem);
            _inGamePacketRegistry.Add(Packet.ObjectDrop, DropItem);
            _inGamePacketRegistry.Add(Packet.ObjectDelete, DeleteItem);
            _inGamePacketRegistry.Add(Packet.SubscribeChannel, SubscribeChannel);
            _inGamePacketRegistry.Add(Packet.ServerMessage, SendMessage);
            _inGamePacketRegistry.Add(Packet.StatsBoost, StatsBoost);
            _inGamePacketRegistry.Add(Packet.ExchangeRequest, ParseRequest);
            _inGamePacketRegistry.Add(Packet.ExchangeLeave, LeaveExchange);
            _inGamePacketRegistry.Add(Packet.ExchangeAccepted, ExchangeAccepted);
            _inGamePacketRegistry.Add(Packet.ExchangeObjectMove, ExchangeMove);
            _inGamePacketRegistry.Add(Packet.ExchangeReady, ExchangeReady);
        }

        public void Parse(string packet)
        {
            var header = packet.Substring(0, 2);

            switch (_packetState)
            {
                case PacketState.Ticket:
                    if (header == Packet.TicketResponse)
                        ParseTicket(packet.Substring(2));
                    break;
                case PacketState.CharactersInfos:
                    if (_charactersPacketRegistry.ContainsKey(header))
                        _charactersPacketRegistry[header](packet.Substring(2));
                    break;
                case PacketState.GameCreate:
                    if (header == Packet.GameCreated)
                        SendCharacterGameInfos();
                    break;
                case PacketState.InGame:
                    if (_inGamePacketRegistry.ContainsKey(header))
                        _inGamePacketRegistry[header](packet.Substring(2));
                    break;
            }
        }

        // Respones functions

        #region HelloGame ^^

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

                    _packetState = PacketState.CharactersInfos;

                    AccountRepository.UpdateAccount(_client.Account.Id);

                    AccountRepository.UpdateCharactersList(_client.Account.Id);

                    _packetState = PacketState.CharactersInfos;

                    _client.SendPackets(Packet.TicketAccepted);
                }
                catch (Exception e)
                {
                    SilverConsole.WriteLine(e.Message, ConsoleColor.Red);
                    Logs.LogWritter(Constant.ErrorsFolder,
                        string.Format("Impossible de charger le compte {0}", account[1]));
                }
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
                    _client.Account.IsNotSubscribed()
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

        #endregion

        #region CreateCharacter

        private void GenerateName(string data)
        {
            _client.SendPackets(string.Format("{0}{1}", Packet.CharacterNameGeneratedResponse,
                Algorithm.GenerateRandomName()));
        }

        private void AddCharacter(string data)
        {
            var charactersNumber =
                DatabaseProvider.AccountCharacters.FindAll(x => x.Account.Id == _client.Account.Id).Count;

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

            if (classe == (int) Character.Class.Pandawa && 
                bool.Parse(Config.Get("Subscription")) &&
                _client.Account.IsNotSubscribed()) 
                return;

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

        #endregion

        #region Gifts

        private void SendGiftsList(string data)
        {
            _client.Account.Gifts = GiftRepository.FindAll(_client.Account.Id).ToList();

            foreach (var gift in _client.Account.Gifts)
            {
                _client.SendPackets(string.Format("{0}1|{1}", Packet.GiftsList, gift));
            }
        }

        private void AddGiftItem(string data)
        {
            var items = _client.Account.Gifts.Find(x => x.Id == int.Parse(data.Split('|')[0])).Items;

            var character = DatabaseProvider.Characters.Find(
                x => x.Id == int.Parse(data.Split('|')[1]));

            foreach (var item in items)
            {
                for (var i = 0; i < item.Quantity; i++)
                {
                    item.Item.Generate(character);
                }
            }

            GiftRepository.RemoveFromAccount(items.First().GiftId, _client.Account.Id);

            _client.SendPackets(Packet.GiftStotedSuccess);
        }

        #endregion

        #region GameInfos

        private void SendCharacterInfos(string data)
        {
            var character = DatabaseProvider.Characters.Find(x => x.Id == int.Parse(data));

            if (character == null) return;

            _client.Character = character;

            _client.SendPackets(string.Format("{0}|{1}",
                Packet.CharacterSelectedResponse,
                character.InfosWheneSelectedCharacter()));

            _packetState = PacketState.GameCreate;
        }

        private void SendCharacterGameInfos()
        {
            if (_client.Character == null) return;

            _client.SendPackets(string.Format("{0}|1|{1}", Packet.GameCreatedResponse, _client.Character.Name));

            SendItemSetsBonnus();

            RefreshCharacterStats();

            _client.SendPackets(Packet.Restriction);

            _client.SendPackets(string.Format("{0}{1}", Packet.Message, Packet.WelcomeToDofus));

            _client.SendPackets(string.Format("{0}+{1}", Packet.SubscribeChannel,
                string.Join("", _client.Character.Channels)));

            LoadMap();

            _packetState = PacketState.InGame;
        }

        private void SendItemSetsBonnus()
        {
            var sets = _client.Character.GetSets();

            foreach (var set in sets)
            {
                SendItemSetBonnus(set);
            }
        }

        private void SendItemSetBonnus(ItemSet set)
        {
            var itemsInTheSameSet = _client.Character.GetAllItemsEquipedInSet(set);

            _client.SendPackets(string.Format("{0}+{1}|{2}|{3}", Packet.ObjectItemSet, set.Id,
                string.Join(",", itemsInTheSameSet.Select(x => x.Id)),
                itemsInTheSameSet.Count > 1
                    ? string.Join(",", set.BonusesDictionary[itemsInTheSameSet.Count].Select(x => x.ToString()))
                    : ""));
        }

        private void RefreshCharacterStats()
        {
            _client.SendPackets(string.Format("{0}{1}|{2}", Packet.CharacterWeight,
                _client.Character.GetCurrentWeight(), _client.Character.GetMaxWeight()));

            _client.SendPackets(string.Format("{0}{1}", Packet.Stats, _client.Character.GetStats()));
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

            if (bool.Parse(Config.Get("Subscription")))
                VerifyMapSubscription();

            _client.Character.Map.SendItemsOnMap();

            _client.SendPackets(Packet.MapLoaded);
        }

        private void VerifyMapSubscription()
        {
            var characterMap = _client.Character.Map;
            var subarea = DatabaseProvider.Subareas.Find(x => x.Id == characterMap.Subarea);

            if (subarea == null) return;

            if (subarea.NeedRegistered)
            {
                if (!_client.Account.IsNotSubscribed()) return;

                _client.Character.SubscriptionRestrictionMap = true;

                _client.SendPackets(string.Format("{0}+10", Packet.SubscriberRestriction));
            }
            else
            {
                _client.Character.SubscriptionRestrictionMap = false;

                _client.SendPackets(string.Format("{0}-", Packet.SubscriberRestriction));
            }
        }

        #endregion

        #region GameActionParser

        private void ParseGameAction(string data)
        {
            switch (data.Substring(0, 1))
            {
                case "0":
                    Move(data);
                    break;
            }
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

        #endregion

        #region DisplaceCharacter

        private void Move(string data)
        {
            if (_client.Character.State != Character.CharacterState.Free &&
                _client.Character.State != Character.CharacterState.OnMove)
                return;

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
            _client.Character.State = Character.CharacterState.OnMove;

            _client.Character.Map.Send(string.Format("{0}0;1;{1};{2}", Packet.GameActions, _client.Character.Id, newPath));
        }

        private void ChangeDestination(string data)
        {
            if (_client.Character.State != Character.CharacterState.OnMove) return;

            var cell = int.Parse(data.Split('|')[1]);

            if (_client.Character.Map.Cells.Contains(cell))
                _client.Character.MapCell = cell;
        }

        private void EndMove()
        {
            if (_client.Character.State != Character.CharacterState.OnMove) return;

            _client.Character.MapCell = _client.Character.CellDestination;
            _client.Character.State = Character.CharacterState.Free;

            var trigger =
                DatabaseProvider.MapTriggers.Find(
                    x => x.Cell == _client.Character.MapCell
                         && x.Map == _client.Character.Map.Id);

            var dropedItem =
                DatabaseProvider.InventoryItems.Find(
                    x => x.Map == _client.Character.Map && x.Cell == _client.Character.MapCell);

            if (dropedItem != null)
                AddDroppedItem(dropedItem);

            if (trigger != null)
                TeleportPlayer(trigger.NewMap, trigger.NewCell);

            _client.SendPackets(Packet.Nothing);
        }

        private void TeleportPlayer(int newMap, int newCell)
        {
            _client.Character.Map.RemoveCharacter(_client.Character);

            _client.SendPackets(string.Format("{0};2;{1};", Packet.GameActions, _client.Character.Id));

            _client.Character.MapCell = newCell;
            _client.Character.Map = DatabaseProvider.Maps.Find(x => x.Id == newMap);

            LoadMap();
        }

        #endregion

        #region DisplaceItems

        private void AddDroppedItem(InventoryItem droppedItem)
        {
            var existItem = InventoryItem.ExistItem(droppedItem, _client.Character);

            _client.Character.Map.Send(string.Format("{0}-{1};{2};0", Packet.CellObject, droppedItem.Cell,
                droppedItem.ItemInfos.Id));

            if (existItem != null)
            {
                existItem.Quantity += droppedItem.Quantity;

                lock (DatabaseProvider.InventoryItems)
                    DatabaseProvider.InventoryItems.Remove(droppedItem);

                _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, existItem.Id,
                    existItem.Quantity));
            }
            else
            {
                droppedItem.Map = null;
                droppedItem.Cell = 0;
                droppedItem.Character = _client.Character;

                InventoryItemRepository.Create(droppedItem, true);

                _client.SendPackets(string.Format("{0}{1}", Packet.ObjectAdd, droppedItem.ItemInfo()));
            }
        }

        private void MoveItem(string data)
        {
            var itemId = int.Parse(data.Split('|')[0]);

            var itemPosition = (StatsManager.Position) int.Parse(data.Split('|')[1]);

            var item =
                DatabaseProvider.InventoryItems.Find(
                    x =>
                        x.Id == itemId &&
                        x.Character.Id == _client.Character.Id);

            if (item == null) return;

            if (itemPosition == StatsManager.Position.None)
            {
                var existItem = InventoryItem.ExistItem(item, item.Character, itemPosition);

                if (existItem != null)
                {
                    var id = item.Id;
                    existItem.Quantity += item.Quantity;

                    InventoryItemRepository.Remove(item, true);

                    _client.SendPackets(string.Format("{0}{1}", Packet.ObjectRemove, id));

                    InventoryItemRepository.Update(existItem);

                    _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, existItem.Id,
                        existItem.Quantity));
                }
                else
                {
                    item.ItemPosition = StatsManager.Position.None;
                    InventoryItemRepository.Update(item);

                    _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectMove, item.Id, (int) itemPosition));
                }

                _client.Character.Stats.RemoveItemStats(item.Stats);

                if (item.ItemInfos.HasSet())
                    _client.Character.Stats.DecreaseItemSetEffect(item);
            }
            else
            {
                if (item.ItemInfos.Level > _client.Character.Level)
                {
                    _client.SendPackets(Packet.ObjectErrorLevel);
                    return;
                }

                if (itemPosition == StatsManager.Position.Anneau1 || itemPosition == StatsManager.Position.Anneau2)
                {
                    if (item.ItemInfos.HasSet())
                    {
                        var itemSet = item.ItemInfos.GetSet();

                        if (_client.Character.CheckIfRingInSetAlredyEquiped(itemSet) == true)
                        {
                            _client.SendPackets(Packet.ObjectErrorRingInSetAlredyEquiped);
                            return;
                        }
                    }
                }

                if (ItemCondition.VerifyIfCharacterMeetItemCondition(_client.Character, item.ItemInfos.Conditions) ==
                    false)
                {
                    _client.SendPackets(string.Format("{0}{1}", Packet.Message, Packet.ConditionToEquipItem));
                    return;
                }

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

                    InventoryItemRepository.Create(newItem, true);

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

                if (item.ItemInfos.HasSet())
                    _client.Character.Stats.IncreaseItemSetEffect(item);

            }

            if (item.ItemInfos.HasSet())
            {
                var set = item.ItemInfos.GetSet();

                SendItemSetBonnus(set);
            }

            RefreshCharacterStats();

            _client.Character.Map.Send(string.Format("{0}{1}|{2}", Packet.ObjectAccessories, _client.Character.Id,
                _client.Character.GetItemsWheneChooseCharacter()));

        }

        private void DropItem(string data)
        {
            var itemId = int.Parse(data.Split('|')[0]);
            var quantity = int.Parse(data.Split('|')[1]);

            var item =
                DatabaseProvider.InventoryItems.Find(
                    x => x.Id == itemId && x.Character.Id == _client.Character.Id && x.Quantity >= quantity);

            if (item == null) return;

            item.Quantity -= quantity;

            var cell = PathFinding.GetNearbyCell(_client.Character.MapCell, _client.Character.Map);

            if (cell == -1)
            {
                _client.SendPackets(string.Format("{0}{1}", Packet.Message, Packet.NotEnoughSpaceToDropItem));
                return;
            }

            if (item.Quantity == 0)
            {
                item.Quantity = quantity;
                item.Map = _client.Character.Map;
                item.Cell = cell;
                item.Character = null;

                InventoryItemRepository.Remove(item, false);

                _client.SendPackets(string.Format("{0}{1}", Packet.ObjectRemove, item.Id));

                _client.Character.Map.Send(string.Format("{0}+{1};{2};0", Packet.CellObject, item.Cell,
                    item.ItemInfos.Id));
            }
            else
            {
                var newItem = item.Copy(quantity: quantity);

                newItem.Map = _client.Character.Map;
                newItem.Cell = cell;
                newItem.Character = null;

                lock (DatabaseProvider.InventoryItems)
                    DatabaseProvider.InventoryItems.Add(newItem);

                _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, item.Id, item.Quantity));

                _client.Character.Map.Send(string.Format("{0}+{1};{2};0", Packet.CellObject, newItem.Cell,
                    item.ItemInfos.Id));
            }

            RefreshCharacterStats();

            if (!item.IsEquiped()) return;

            _client.Character.Stats.RemoveItemStats(item.Stats);

            _client.Character.Map.Send(string.Format("{0}{1}|{2}", Packet.ObjectAccessories, _client.Character.Id,
                _client.Character.GetItemsWheneChooseCharacter()));
        }


        private void DeleteItem(string data)
        {
            var itemId = int.Parse(data.Split('|')[0]);
            var quantity = int.Parse(data.Split('|')[1]);

            var item =
                DatabaseProvider.InventoryItems.Find(
                    x => x.Id == itemId && x.Character.Id == _client.Character.Id && x.Quantity >= quantity);

            if (item == null) return;

            item.Quantity -= quantity;

            if (item.Quantity <= 0)
            {
                _client.SendPackets(string.Format("{0}{1}", Packet.ObjectRemove, item.Id));

                InventoryItemRepository.Remove(item, true);
            }
            else
            {
                item.Quantity = quantity;

                _client.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, item.Id, item.Quantity));

                InventoryItemRepository.Update(item);
            }

            RefreshCharacterStats();

            if (!item.IsEquiped()) return;

            _client.Character.Stats.RemoveItemStats(item.Stats);

            _client.Character.Map.Send(string.Format("{0}{1}|{2}", Packet.ObjectAccessories, _client.Character.Id,
                _client.Character.GetItemsWheneChooseCharacter()));
        }

        #endregion

        #region Chat

        private void SubscribeChannel(string data)
        {
            if (!Channel.Headers.Contains(data[1])) return;

            var state = data[0];
            var header = (Channel.ChannelHeader) data[1];

            switch (state)
            {
                case '+':
                    _client.Character.AddChannel(header);
                    break;
                case '-':
                    _client.Character.RemoveChannel(header);
                    break;
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

        #endregion

        #region CharacterStats

        private void StatsBoost(string data)
        {
            var baseStats = int.Parse(data);

            if (baseStats < 10 || baseStats > 15) return;

            _client.Character.BoostStats((Character.BaseStats) baseStats);

            _client.SendPackets(string.Format("{0}{1}", Packet.Stats, _client.Character.GetStats()));
        }

        #endregion

        #region ExchangeRequests

        private void ParseRequest(string data)
        {
            var id = int.Parse(data.Split('|')[0]);

            switch (id)
            {
                case 0:
                    break;
                case 1:
                    StartExchangeWithPlayer(data);
                    break;
            }
        }

        private void StartExchangeWithPlayer(string data)
        {
            if (_client.Character.State != Character.CharacterState.Free
                && _client.Character.State != Character.CharacterState.OnMove)
                return;

            var requestId = data.Split('|')[0];
            var receiverId = int.Parse(data.Split('|')[1]);

            var receiverCharacter = DatabaseProvider.Characters.Find(x => x.Id == receiverId);

            if (receiverCharacter == null)
            {
                _client.SendPackets(Packet.CannotExchangeWithThisPlayer);
                return;
            }

            var receiverClient = GameServer.Clients.Find(x => x.Character == receiverCharacter);

            if (receiverClient == null)
            {
                _client.SendPackets(Packet.CannotExchangeWithThisPlayer);
                return;
            }

            if (receiverClient.Character.Map != _client.Character.Map)
            {
                _client.SendPackets(Packet.CannotExchangeWithThisPlayer);
                return;
            }

            if (receiverClient.Character.State == Character.CharacterState.OnExchange)
            {
                _client.SendPackets(Packet.PlayerIsAlredyOnExchange);
                return;
            }

            if (receiverClient.Character.State != Character.CharacterState.Free)
            {
                _client.SendPackets(Packet.CannotExchangeWithThisPlayer);
                return;
            }

            receiverCharacter.State = Character.CharacterState.OnExchange;
            receiverCharacter.ExchangeWithCharacter = _client.Character;

            _client.Character.State = Character.CharacterState.OnExchange;
            _client.Character.ExchangeWithCharacter = receiverCharacter;

            receiverClient.SendPackets(string.Format("{0}{1}|{2}|{3}", Packet.ExchangeRequestValidated,
                _client.Character.Id, receiverId, requestId));
            _client.SendPackets(string.Format("{0}{1}|{2}|{3}", Packet.ExchangeRequestValidated, _client.Character.Id,
                receiverId, requestId));
        }

        private void ExchangeAccepted(string data)
        {
            if (_client.Character.State != Character.CharacterState.OnExchange) return;

            var receiverCharacter = _client.Character.ExchangeWithCharacter;

            // TODO : other type of exchange here, maybe do a switch later...
            if (receiverCharacter == null) return;

            var receiverClient = GameServer.Clients.Find(x => x.Character == receiverCharacter);

            if (receiverClient == null) return;

            ExchangeManager.CreateExchangeSession(_client.Character, receiverClient.Character);

            receiverClient.SendPackets(string.Format("{0}{1}", Packet.ExchangeCreated, "1"));

            _client.SendPackets(string.Format("{0}{1}", Packet.ExchangeCreated, "1"));
        }

        private void LeaveExchange(string data)
        {
            if (_client.Character.State != Character.CharacterState.OnExchange) return;

            var receiverCharacter = _client.Character.ExchangeWithCharacter;

            if (receiverCharacter != null)
                _client.Character.LeaveExchangeWithPlayer();

            var receiverClient = GameServer.Clients.Find(x => x.Character == receiverCharacter);

            var exchangeSession =
                ExchangeManager.Exchanges.Find(
                    x => x.FirstTrader == _client.Character && x.SecondTrader == receiverClient.Character
                         || x.FirstTrader == receiverClient.Character && x.SecondTrader == _client.Character);

            if (exchangeSession != null)
            {
                ExchangeManager.CloseExchangeSession(exchangeSession);
            }

            receiverClient.SendPackets(Packet.ExchangeLeave);
            _client.SendPackets(Packet.ExchangeLeave);
        }

        private void ExchangeMove(string data)
        {
            var id = data[0];

            var receiverCharacter = _client.Character.ExchangeWithCharacter;

            var receiverClient = GameServer.Clients.Find(x => x.Character == receiverCharacter);

            if (receiverClient == null) return;

            var exchangeSession =
                ExchangeManager.Exchanges.Find(
                    x => x.FirstTrader == _client.Character && x.SecondTrader == receiverClient.Character
                         || x.FirstTrader == receiverClient.Character && x.SecondTrader == _client.Character);

            if (exchangeSession == null) return;

            switch (id)
            {
                case 'O':
                    ExchangeMoveObject(data.Substring(1), receiverClient, exchangeSession);
                    break;
                case 'G':
                    ExchangeMoveKamas(data.Substring(1), receiverClient, exchangeSession);
                    break;
            }
        }

        private void ExchangeMoveObject(string data, GameClient receiverClient, Exchange exchangeSession)
        {
            if (_client.Character.State != Character.CharacterState.OnExchange) return;

            var state = data[0];
            var itemId = int.Parse(data.Substring(1).Split('|')[0]);
            var quantity = int.Parse(data.Substring(1).Split('|')[1]);

            var item =
                DatabaseProvider.InventoryItems.Find(
                    x => x.Character == _client.Character && x.Id == itemId && x.Quantity >= quantity);

            if (item == null) return;

            if (item.ItemPosition != StatsManager.Position.None) return;

            switch (state)
            {
                case '+':
                    exchangeSession.AddItem(_client.Character, item, quantity, _client, receiverClient);
                    break;
                case '-':
                    exchangeSession.RemoveItem(_client.Character, item, quantity, _client, receiverClient);
                    break;
            }
        }

        private void ExchangeMoveKamas(string data, GameClient receiverClient, Exchange exchangeSession)
        {
            if (_client.Character.State != Character.CharacterState.OnExchange) return;

            var kamas = int.Parse(data);

            if (_client.Character.Kamas >= kamas)
                exchangeSession.AddKamas(_client.Character, kamas, _client, receiverClient);
        }

        private void ExchangeReady(string data)
        {
            if (_client.Character.State != Character.CharacterState.OnExchange) return;

            var receiverCharacter = _client.Character.ExchangeWithCharacter;

            var receiverClient = GameServer.Clients.Find(x => x.Character == receiverCharacter);

            if (receiverClient == null) return;

            var exchangeSession =
                ExchangeManager.Exchanges.Find(
                    x => x.FirstTrader == _client.Character && x.SecondTrader == receiverClient.Character
                         || x.FirstTrader == receiverClient.Character && x.SecondTrader == _client.Character);

            if (exchangeSession == null) return;

            var finishedExchange = exchangeSession.Accepted(_client.Character, _client, receiverClient);

            if (!finishedExchange) return;

            exchangeSession.Swap();

            ExchangeManager.CloseExchangeSession(exchangeSession);

            receiverClient.Character.State = Character.CharacterState.Free;
            receiverClient.Character.ExchangeWithCharacter = null;

            _client.Character.State = Character.CharacterState.Free;
            _client.Character.ExchangeWithCharacter = null;
        }

        #endregion

        private enum PacketState
        {
            Ticket,
            CharactersInfos,
            GameCreate,
            InGame
        }
    }
}