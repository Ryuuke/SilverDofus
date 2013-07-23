﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SilverGame.Models;
using SilverGame.Services;
using Client = SilverGame.Network.Game.GameClient;

namespace SilverGame.Network.Game.GameParser
{
    class GameParser
    {
        private readonly Client _client;

        private delegate void Response(string data);

        private static Dictionary<string, Response> _packetRegistry; 

        public GameParser(Client client)
        {
            _client = client;

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

            if (DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", null).AddSeconds(Constant.TicketTimeExpiredInterval) < DateTime.Now)
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
                        Connected = bool.Parse(account[6]),
                        GmLevel = int.Parse(account[7]),
                        BannedUntil = account[8] == "" ? (DateTime?)null : DateTime.Parse(account[8].ToString(System.Globalization.CultureInfo.InvariantCulture)),
                        Subscription = account[9] == "" ? (DateTime?)null : DateTime.Parse(account[9].ToString(System.Globalization.CultureInfo.InvariantCulture)),
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Logs.LogWritter(Constant.ErrorsFolder, string.Format("Impossible de charger le compte {0}", account[1]));
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
            _client.SendPackets(Packet.CharactersListResponse + "0");
        }

        private void GenerateName(string data)
        {
            _client.SendPackets(string.Format("{0}{1}", Packet.CharacterNameGeneratedResponse, Algorithm.GenerateRandomName()));
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
      
            if (!Database.Characters.Exist(name) && name.Length >= 3 && name.Length <= 20)
            {
                var reg = new Regex("^[a-zA-Z-]+$");

                if (reg.IsMatch(name) && name.Count(c => c == '-') < 3)
                {
                    if (classe >= 1 && classe <= 12 && (sex == 1 || sex == 0))
                    {
                        Database.Characters.CreateNewCharacter(name, classe, sex, color1, color2, color3, _client.Account.Id);  
                        _client.SendPackets(Packet.CreationSuccess);
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
    }
}
