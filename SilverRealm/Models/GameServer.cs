namespace SilverRealm.Models
{
    class GameServer
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Key { get; set; }
        public int State { get; set; }

        public override string ToString()
        {
            return string.Format("{0};{1};0;1", Id, State);
        }
    }
}
