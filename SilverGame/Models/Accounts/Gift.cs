using System.Collections.Generic;
using SilverGame.Models.Items;

namespace SilverGame.Models.Accounts
{
    class Gift
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public IEnumerable<ItemInfos> Items { get; set; }
    }
}
