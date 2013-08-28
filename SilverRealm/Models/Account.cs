using System;

namespace SilverRealm.Models
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

        public bool IsNotSubscribed()
        {
            return Subscription == null || Subscription.Value < DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                Id, Username, Password, Pseudo, Question, Reponse, Connected, GmLevel, BannedUntil, Subscription);
        }
    }
}
