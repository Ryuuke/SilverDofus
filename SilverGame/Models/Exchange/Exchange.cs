using System;
using System.Collections.Generic;
using System.Transactions;
using SilverGame.Database.Repository;
using SilverGame.Models.Characters;
using SilverGame.Models.Items;
using SilverGame.Network.Game;
using SilverGame.Services;

namespace SilverGame.Models.Exchange
{
    internal class Exchange
    {
        private const int TimeInvtervalToAccept = 2000;

        public Character FirstTrader { get; private set; }
        private readonly List<InventoryItem> _firstTraderItems;
        private int FirstKamas { get; set; }
        private bool _firstValidated;

        public Character SecondTrader { get; set; }
        private readonly List<InventoryItem> _secondTraderItems;
        private int SecondKamas { get; set; }
        private bool _secondValidated;

        private DateTime _timer;

        public Exchange(Character firstTrader, Character secondTrader)
        {
            FirstTrader = firstTrader;
            SecondTrader = secondTrader;

            _firstTraderItems = new List<InventoryItem>();
            _secondTraderItems = new List<InventoryItem>();

            _timer = DateTime.Now;
        }

        public void AddItem(Character character, InventoryItem item, int quantity, GameClient client,
            GameClient receiverClient)
        {
            _timer = DateTime.Now;

            UnLockExchangeCases(client, receiverClient); // Update exchange board

            InventoryItem existItem;

            if (FirstTrader == character)
            {
                existItem =
                    _firstTraderItems.Find(
                        x =>
                            x.Character == item.Character && x.ItemPosition == item.ItemPosition &&
                            x.ItemInfos == item.ItemInfos && x.Stats == item.Stats);
            }
            else
            {
                existItem =
                    _secondTraderItems.Find(
                        x =>
                            x.Character == item.Character && x.ItemPosition == item.ItemPosition &&
                            x.ItemInfos == item.ItemInfos && x.Stats == item.Stats);
            }

            if (existItem != null)
            {
                existItem.Quantity += quantity;

                client.SendPackets(string.Format("{0}+{1}|{2}", Packet.ExchangeObjectLocalObjectMove, item.Id,
                    existItem.Quantity));

                receiverClient.SendPackets(string.Format("{0}+{1}", Packet.ExchangeObjectDistantObjectMove,
                    item.ToExchangeFormat(existItem.Quantity)));
            }
            else
            {
                var newItem = item.Copy(quantity: quantity);

                if (character == FirstTrader)
                    _firstTraderItems.Add(newItem);
                else
                    _secondTraderItems.Add(newItem);

                client.SendPackets(string.Format("{0}+{1}|{2}", Packet.ExchangeObjectLocalObjectMove, item.Id,
                    newItem.Quantity));

                receiverClient.SendPackets(string.Format("{0}+{1}", Packet.ExchangeObjectDistantObjectMove,
                    item.ToExchangeFormat(newItem.Quantity)));
            }
        }

        public void RemoveItem(Character character, InventoryItem item, int quantity, GameClient client,
            GameClient receiverClient)
        {
            _timer = DateTime.Now;

            UnLockExchangeCases(client, receiverClient);

            InventoryItem existItem;

            if (character == FirstTrader)
            {
                existItem =
                    _firstTraderItems.Find(
                        x =>
                            x.Character == item.Character && x.ItemPosition == item.ItemPosition &&
                            x.ItemInfos == item.ItemInfos && x.Stats == item.Stats);
            }
            else
            {
                existItem =
                    _secondTraderItems.Find(
                        x =>
                            x.Character == item.Character && x.ItemPosition == item.ItemPosition &&
                            x.ItemInfos == item.ItemInfos && x.Stats == item.Stats);
            }

            if (existItem == null) return;

            if (existItem.Quantity >= quantity)
                existItem.Quantity -= quantity;

            if (existItem.Quantity == 0)
            {
                client.SendPackets(string.Format("{0}-{1}", Packet.ExchangeObjectLocalObjectMove, item.Id));

                receiverClient.SendPackets(string.Format("{0}-{1}", Packet.ExchangeObjectDistantObjectMove, item.Id));

                if (FirstTrader == character)
                    _firstTraderItems.Remove(existItem);
                else
                    _secondTraderItems.Remove(existItem);
            }
            else
            {
                client.SendPackets(string.Format("{0}+{1}|{2}", Packet.ExchangeObjectLocalObjectMove, item.Id,
                    existItem.Quantity));

                receiverClient.SendPackets(string.Format("{0}+{1}", Packet.ExchangeObjectDistantObjectMove,
                    item.ToExchangeFormat(existItem.Quantity)));
            }
        }


        public void AddKamas(Character character, int kamas, GameClient client, GameClient receiverClient)
        {
            _timer = DateTime.Now;

            UnLockExchangeCases(client, receiverClient);

            if (FirstTrader == character)
                FirstKamas = kamas;
            else
                SecondKamas = kamas;

            client.SendPackets(string.Format("{0}{1}", Packet.ExchangeObjectLocalKamasMove, kamas));
            receiverClient.SendPackets(string.Format("{0}{1}", Packet.ExchangeObjectDistantKamasMove, kamas));
        }

        public bool Accepted(Character character, GameClient client, GameClient receiverClient)
        {
            if (DateTime.Now < _timer.AddMilliseconds(TimeInvtervalToAccept))
                return false;

            if (character == FirstTrader)
            {
                _firstValidated = _firstValidated == false;

                LockExchangeCases(client, receiverClient, FirstTrader, _firstValidated);
            }
            else
            {
                _secondValidated = _secondValidated == false;

                LockExchangeCases(client, receiverClient, SecondTrader, _secondValidated);
            }

            return _firstValidated && _secondValidated;
        }

        private void LockExchangeCases(GameClient client, GameClient receiverClient, Character character, bool validated)
        {
            client.SendPackets(string.Format("{0}{1}{2}", Packet.ExchangeReady, validated ? "1" : "0",
                    character.Id));

            receiverClient.SendPackets(string.Format("{0}{1}{2}", Packet.ExchangeReady, validated ? "1" : "0",
                character.Id));
        }

        private void UnLockExchangeCases(GameClient client, GameClient receiverClient)
        {
            _firstValidated = false;
            _secondValidated = false;

            client.SendPackets(string.Format("{0}{1}{2}", Packet.ExchangeReady, "0",
                    FirstTrader.Id));

            receiverClient.SendPackets(string.Format("{0}{1}{2}", Packet.ExchangeReady, "0",
                FirstTrader.Id));

            client.SendPackets(string.Format("{0}{1}{2}", Packet.ExchangeReady, "0",
                    SecondTrader.Id));

            receiverClient.SendPackets(string.Format("{0}{1}{2}", Packet.ExchangeReady, "0",
                SecondTrader.Id));
        }

        public void Swap()
        {
            var firstTraderClient = GameServer.Clients.Find(x => x.Character == FirstTrader);
            var secondTraderClient = GameServer.Clients.Find(x => x.Character == SecondTrader);

            if (firstTraderClient == null || secondTraderClient == null)
                return;

            try
            {
                
                using (var transaction = new TransactionScope())
                {
                    foreach (var firstTraderItem in _firstTraderItems)
                        EchangeItem(firstTraderClient, secondTraderClient, firstTraderItem, FirstTrader);
                    
                    foreach (var secondTraderItem in _secondTraderItems)
                        EchangeItem(firstTraderClient, secondTraderClient, secondTraderItem, SecondTrader);
                    
                    ExchangeKamas();

                    transaction.Complete();
                }

                firstTraderClient.SendPackets(string.Format("{0}{1}|{2}", Packet.CharacterWeight,
                    firstTraderClient.Character.GetCurrentWeight(), firstTraderClient.Character.GetMaxWeight()));

                secondTraderClient.SendPackets(string.Format("{0}{1}|{2}", Packet.CharacterWeight,
                    secondTraderClient.Character.GetCurrentWeight(), secondTraderClient.Character.GetMaxWeight()));

                firstTraderClient.SendPackets(string.Format("{0}{1}", Packet.Stats,
                    firstTraderClient.Character.GetStats()));
                secondTraderClient.SendPackets(string.Format("{0}{1}", Packet.Stats,
                    secondTraderClient.Character.GetStats()));

                firstTraderClient.SendPackets(Packet.ExchangeLeaveSucess);
                secondTraderClient.SendPackets(Packet.ExchangeLeaveSucess);
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(e.Message, ConsoleColor.Red);

                firstTraderClient.SendPackets(Packet.ExchangeLeave);
                secondTraderClient.SendPackets(Packet.ExchangeLeave);
            }
        }

        private void EchangeItem(GameClient firstTraderClient, GameClient secondTraderClient, InventoryItem item,
            Character trader)
        {
            var existItemSecondTrader = InventoryItem.ExistItem(item, SecondTrader);
            var existItemFirstTrader = InventoryItem.ExistItem(item, FirstTrader);

            if (FirstTrader == trader)
            {
                RemoveTraderItem(existItemFirstTrader, firstTraderClient, item);
                CreateTraderItem(SecondTrader, existItemSecondTrader, secondTraderClient, item);
            }
            else
            {
                RemoveTraderItem(existItemSecondTrader, secondTraderClient, item);
                CreateTraderItem(FirstTrader, existItemFirstTrader, firstTraderClient, item);
            }
        }

        private void RemoveTraderItem(InventoryItem existItemTrader, GameClient traderClient, InventoryItem item)
        {
            existItemTrader.Quantity -= item.Quantity;

            if (existItemTrader.Quantity <= 0)
            {
                traderClient.SendPackets(string.Format("{0}{1}", Packet.ObjectRemove, existItemTrader.Id));

                InventoryItemRepository.Remove(existItemTrader, true);
            }
            else
            {
                traderClient.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, existItemTrader.Id,
                    existItemTrader.Quantity));

                InventoryItemRepository.Update(existItemTrader);
            }
        }

        private void CreateTraderItem(Character trader, InventoryItem existItemTrader, GameClient traderClient,
            InventoryItem item)
        {
            if (existItemTrader != null)
            {
                existItemTrader.Quantity += item.Quantity;

                traderClient.SendPackets(string.Format("{0}{1}|{2}", Packet.ObjectQuantity, existItemTrader.Id,
                    existItemTrader.Quantity));

                InventoryItemRepository.Update(existItemTrader);
            }
            else
            {
                var newItem = item.Copy(quantity: item.Quantity);

                newItem.Character = trader;

                traderClient.SendPackets(string.Format("{0}{1}", Packet.ObjectAdd, newItem.ItemInfo()));

                InventoryItemRepository.Create(newItem, true);
            }
        }

        private void ExchangeKamas()
        {
            FirstTrader.Kamas -= FirstKamas;
            SecondTrader.Kamas -= SecondKamas;

            FirstTrader.Kamas += SecondKamas;
            SecondTrader.Kamas += FirstKamas;
        }
    }
}