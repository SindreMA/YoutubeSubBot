using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using YoutubeSubBot.DTO;

namespace TemplateBot
{
    class Timer
    {
        public System.Threading.Timer _timer;

        private DiscordSocketClient _client;
        private CommandService _service;
        public static List<YoutuberDTO> Youtubers = new List<YoutuberDTO>();
        public static List<ulong> DontAlertUsers = new List<ulong>();
        public List<AlertedVideoDTO> AlertedVideos = new List<AlertedVideoDTO>();
        public Timer(DiscordSocketClient client)
        {
            _client = client;
            _timer = new System.Threading.Timer(Callback, true, 1000, System.Threading.Timeout.Infinite);

            YoutuberDTO i1 = new YoutuberDTO();
            i1.YoutuberID = "jacksfilms";
            List<ulong> listitem = new List<ulong>();
            listitem.Add(170605899189190656);
            i1.Subscribers = listitem;

            YoutuberDTO i2 = new YoutuberDTO();
            i2.YoutuberID = "ExplosmEntertainment";
            List<ulong> listitem2 = new List<ulong>();
            listitem2.Add(170605899189190656);
            i2.Subscribers = listitem2;

            YoutuberDTO i3 = new YoutuberDTO();
            i3.YoutuberID = "testedcom";
            List<ulong> listitem3 = new List<ulong>();
            listitem3.Add(170605899189190656);
            i3.Subscribers = listitem3;


            Youtubers.Add(i1);
            Youtubers.Add(i2);
            Youtubers.Add(i3);

        }
        private void Callback(Object state)
        {
            TimerEvent();
            _timer.Change(30000, Timeout.Infinite);
        }
        private void TimerEvent()
        {
            List<YoutubeLinkDTO> AllNewVideos = new List<YoutubeLinkDTO>();
            foreach (var item in Youtubers)
            {
                var List = CheckNewVideoAsync(item.YoutuberID);
                YoutubeLinkDTO link = new YoutubeLinkDTO();
                link.ListOfVideos = List;
                link.Youtuber = item;
                if (link.ListOfVideos.Count != 0)
                {
                    AllNewVideos.Add(link);
                }
            }
            foreach (var item in AllNewVideos)
            {
                foreach (var item2 in item.ListOfVideos)
                {
                    if (!AlertedVideos.Exists(x => x.Video.VideoLink == item2.VideoLink))
                    {
                        foreach (var item3 in item.Youtuber.Subscribers)
                        {
                            if (!DontAlertUsers.Exists(x => x == item3))
                            {


                                try
                                {
                                    var user = _client.GetUser(item3);
                                    var channel = user.GetOrCreateDMChannelAsync().Result;
                                    channel.SendMessageAsync("", false, SimpleEmbed(
                                        new Color(1f, 1f, 1f),
                                        "New Video from " + item2.ChannelName,
                                        "**Title: **" + item2.Title + Environment.NewLine +
                                        "**Video length:** " + item2.Lenght + Environment.NewLine +
                                        "**Views:** " + item2.Views + Environment.NewLine,
                                        item2.VideoLink,

                                        item2.Thumbnail,
                                        item2.Youtubericon


                                        ));
                                }
                                catch (Exception)
                                {

                                    var channel = _client.GetChannel(item3) as SocketTextChannel;
                                    channel.SendMessageAsync("", false, SimpleEmbed(
                                        new Color(1f, 1f, 1f),
                                        "New Video from " + item2.ChannelName,
                                        "**Title: **" + item2.Title + Environment.NewLine +
                                        "**Video length:** " + item2.Lenght + Environment.NewLine +
                                        "**Views:** " + item2.Views + Environment.NewLine,
                                        item2.VideoLink,
                                        item2.Thumbnail,
                                        item2.Youtubericon
                                        ));
                                }
                            }
                            Thread.Sleep(1000);
                        }
                        Program.Log("New video from " + item2.ChannelName + " alerted to " + item.Youtuber.Subscribers.Count, ConsoleColor.Red);
                        AlertedVideoDTO Alert = new AlertedVideoDTO();
                        Alert.DatetimeCreated = DateTime.Now;
                        Alert.Video = item2;
                        AlertedVideos.Add(Alert);
                    }
                }
            }
        }
        private List<YoutubeVideoDTO> CheckNewVideoAsync(string Channel)
        {
            string URL = "https://www.youtube.com/user/" + Channel + "/videos";
            //Find all videos that have been posted within 1 hour from channel

            int i = 1;
            HtmlDocument doc = new HtmlDocument();
            Thread.Sleep(100);
            HtmlWeb hw = new HtmlWeb();
            Thread.Sleep(100);
            doc = hw.Load(URL);
            Thread.Sleep(100);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//*[@id=""channels-browse-content-grid""]");
            HtmlNodeCollection Iconnodes = doc.DocumentNode.SelectNodes("//*[@id=\"c4-header-bg-container\"]/a/img");
            HtmlNodeCollection ChannelName = doc.DocumentNode.SelectNodes("//*[@id=\"c4-primary-header-contents\"]/div/div/div[1]/h1/span/span/span/a");

            Thread.Sleep(100);
            List<YoutubeVideoDTO> VideoList = new List<YoutubeVideoDTO>();

            HtmlNodeCollection ds = null;
            try
            {
                foreach (var item2 in ds = nodes[0].ChildNodes)
                {
                    if (item2.Name == "li")
                    {
                        YoutubeVideoDTO Video = new YoutubeVideoDTO();
                        int uploaded = 0;
                        string timesinceupload = item2.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[1].InnerHtml;
                        if (timesinceupload.Contains("år"))
                        {
                            uploaded = 100000;
                        }
                        else if (timesinceupload.Contains("måneder"))
                        {
                            uploaded = 100000;
                        }
                        else if (timesinceupload.Contains("dager"))
                        {
                            uploaded = 100000;
                        }
                        else if (timesinceupload.Contains("timer"))
                        {
                            uploaded = 100000;
                        }
                        else if (timesinceupload.Contains("minutter"))
                        {
                            uploaded = int.Parse(timesinceupload.Split(' ')[1]) * 60;
                        }
                        else if (timesinceupload.Contains("sekunder"))
                        {
                            uploaded = int.Parse(timesinceupload.Split(' ')[1]) * 60;
                        }
                        string link = "http://youtube.com" + StringFinder(item2.ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerHtml, "<a href=\"", "\" class=\"yt-uix-sessionlink\"");
                        string views = item2.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[3].ChildNodes[1].ChildNodes[0].InnerHtml.Replace("sett ", "").Replace(" ganger", "");
                        string lenght = item2.ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[2].InnerText;
                        string title = StringFinder(item2.ChildNodes[1].ChildNodes[1].InnerHtml, " dir=\"ltr\" title=\"", "\" aria-describedby=\"description-id-").Replace("&#39;", "'");
                        string dsds = item2.ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[1].InnerHtml;
                        string thumbnail = "https://i.ytimg.com" + StringFinder(dsds, "src=\"https://i.ytimg.com", "\" aria-hidden=\"").Replace("==&amp;rs=", "==&rs=").Split(' ')[0].Replace("\"", "").Replace("&amp;", "&");
                        string icon = StringFinder(Iconnodes[0].OuterHtml, "<img class=\"channel-header-profile-image\" src=\"", "\" title=\"");
                        string name = ChannelName[0].InnerText;
                        if (uploaded != 0 && uploaded < 3600  && !AlertedVideos.Exists(x => x.Video.VideoLink == link))
                        {
                            //IF video alerted before
                            Video.ChannelName = name;
                            Video.Youtubericon = icon;
                            Video.Thumbnail = thumbnail;
                            Video.Views = views;
                            Video.Lenght = lenght;
                            Video.Title = title;
                            Video.VideoLink = link;
                            Video.YoutuberID = Channel;
                            VideoList.Add(Video);
                        }

                    }
                }
            }
            catch (Exception)
            {
            }
            return VideoList;

        }
     
        public static string StringFinder(string Input, string Split, string split2)
        {
            Match match = Regex.Match(Input, Split + @"(?<text>[^\]]*)" + split2, RegexOptions.IgnoreCase);
            string value = "";
            if (match.Success)
            {
                value = match.Groups[1].Value; //result here
            }
            return value;
        }
        public static Embed SimpleEmbed(Color c, string title, string description, string Url, string image, string Thubnail)
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.ThumbnailUrl = Thubnail;
            eb.WithColor(c);
            eb.Title = title;
            eb.Url = Url;
            //eb.ImageUrl = image;
            eb.WithDescription(description);
            return eb.Build();
        }
        public class Grabby
        {
            public string Grab(string url)
            {
                var sd = "index.js " + url;
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {


                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "phantomjs.exe",
                    Arguments = sd
                };

                process.StartInfo = startInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
                //var grabby = new Grabby();
            }
        }
    }
}
