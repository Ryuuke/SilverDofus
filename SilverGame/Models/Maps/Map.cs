using System;
using System.Collections.Generic;
using System.Linq;
using SilverGame.Models.Characters;
using SilverGame.Network.Game;
using SilverGame.Services;

namespace SilverGame.Models.Maps
{
    class Map
    {
        public int Id { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Subarea { get; set; }
        public string MapData { get; set; }
        public string Key { get; set; }
        public string Time { get; set; }
        public bool NeedRegister { get; set; }
        private readonly List<Character> _characters = new List<Character>();
        public List<int> Cells = new List<int>();

        public void AddCharacter(Character character)
        {
            lock (_characters)
                _characters.Add(character);
        }

        public void RemoveCharacter(Character character)
        {
            lock (_characters)
                _characters.Remove(character);

            Send(string.Format("{0}|-{1}", Packet.Movement, character.Id));
        }

        public void Send(string packet)
        {
            lock (GameServer.Lock)
            {
                foreach (var character in _characters)
                {
                    GameServer.Clients.Find(x => x.Character == character).SendPackets(packet);
                }
            }
        }

        public List<int> UncompressDatas()
        {
            var newList = new List<int>();

            const string hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            var data = DecypherData(MapData, "");

            for (var i = 0; i < data.Length; i += 10)
            {
                var currentCell = data.Substring(i, 10);
                byte[] cellInfo = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (var i2 = currentCell.Length - 1; i2 >= 0; i2--)
                    cellInfo[i2] = (byte)hash.IndexOf(currentCell[i2]);

                var type = (cellInfo[2] & 56) >> 3;

                if (type != 0)
                    newList.Add(i / 10);
            }

            return newList;
        }

        private static string DecypherData(string data, string decryptKey)
        {
            try
            {
                var result = string.Empty;

                if (decryptKey == "") return data;

                decryptKey = PrepareKey(decryptKey);
                var checkSum = CheckSum(decryptKey) * 2;

                for (int i = 0, k = 0; i < data.Length; i += 2)
                    result += (char)(int.Parse(data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber) ^ decryptKey[(k++ + checkSum) % decryptKey.Length]);

                return Uri.UnescapeDataString(result);
            }
            catch { return ""; }
        }

        private static string PrepareKey(string key)
        {
            var keyResult = "";

            for (var i = 0; i < key.Length; i += 2)
                keyResult += Convert.ToChar(int.Parse(key.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));

            return Uri.UnescapeDataString(keyResult);
        }

        private static int CheckSum(string data)
        {
            var result = data.Sum(t => t%16);

            return result % 16;
        }

        public string DisplayChars()
        {
            return _characters.Aggregate(string.Empty, (current, character) => current + string.Format("|+{0}", character.DisplayChar()));
        }
    }
}
