using System;
using System.Collections.Generic;
using SilverGame.Database.Repository;
using SilverGame.Models.Gifts;

namespace SilverGame.Models.Accounts
{
    class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Pseudo { get; set; }
        public string Question { get; set; }
        public string Reponse { get; set; }
        public int GmLevel { get; set; }
        public DateTime? BannedUntil { get; set; }
        public DateTime? Subscription { get; set; }
        public List<Gift> Gifts { get; set; } 

        public bool IsNotSubscribed()
        {
            return Subscription == null || Subscription.Value < DateTime.Now;
        }

        public void Disconnect()
        {
            AccountRepository.UpdateAccount(Id, false);
        }
    }
}