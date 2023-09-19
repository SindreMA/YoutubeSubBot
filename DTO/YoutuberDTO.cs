using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeSubBot.DTO
{
    public class YoutuberDTO
    {
        public string YoutuberID { get; set; }
        public List<ulong> Subscribers { get; set; }
        public string Image { get; set; }

    }
}
