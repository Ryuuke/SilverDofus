using System.Globalization;

namespace SilverGame.Models.Chat
{
    class Channel
    {

        public static readonly char[] Headers = {'i', '*', '#', '%', '!', '?', ':', '@'};

        public ChannelHeader Header;

        public override string ToString()
        {
            return ((char) Header).ToString(CultureInfo.InvariantCulture);
        }

        public enum ChannelHeader
        {
            InfomationChannal = 'i',
            DefaultChannel = '*',
            PrivateChannel = '#',
            GuildChannel = '%',
            AlignmentChannel = '!',
            RecruitmentChannel = '?',
            BusinessChannel = ':',
            AdminChannel = '@',
            UndefinedChannel = '¤',
        }
    }
}