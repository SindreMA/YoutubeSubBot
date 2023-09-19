using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeSubBot.DTO
{
    public class AlertedVideoDTO
    {
        public YoutubeVideoDTO Video { get; set; }
        public DateTime DatetimeCreated { get; set; }

    }
}
