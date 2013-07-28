﻿using System;

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
        public bool Connected { get; set; }
        public int GmLevel { get; set; }
        public DateTime? BannedUntil { get; set; }
        public DateTime? Subscription { get; set; }
    }
}