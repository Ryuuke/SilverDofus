using System.Collections.Generic;
using System.Linq;
using SilverGame.Models.Items;

namespace SilverGame.Models.Gifts
{
    class Gift
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public List<GiftItems> Items { get; set; }

        public override string ToString()
        {
            var gift = string.Format("{0}|{1}|{2}|{3}|",
                Id, Title, Description, PictureUrl);

            for(var i=0; i < Items.Count() ; i++)
            {
                gift += string.Format("{0}~{1};", i+1, Items.ElementAt(i).GetGiftFormat());
            }

            return gift;
        }
    }
}
