using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeSubBot.DTO
{
    public class UserDTO
    {
        public ulong DiscordId { get; set; }
        public List<int> SubscribedTo { get; set; }

    }
}
