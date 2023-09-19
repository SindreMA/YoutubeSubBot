using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeSubBot.DTO
{
     public class YoutubeLinkDTO
    {
        public List<YoutubeVideoDTO> ListOfVideos { get; set; }
        public YoutuberDTO Youtuber { get; set; }
    }
}
