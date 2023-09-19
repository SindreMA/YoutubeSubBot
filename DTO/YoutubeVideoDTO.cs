using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeSubBot.DTO
{
    public class YoutubeVideoDTO
    {
        public string YoutuberID { get; set; }
        public string VideoLink { get; set; }
        public string Lenght { get; set; }
        public string Title { get; set; }
        public string Views { get; set; }
        public string Thumbnail { get; set; }
        public string Youtubericon { get; set; }
        public string ChannelName { get; set; }
        public UserDTO Subscribers { get; set; }
    }
}
