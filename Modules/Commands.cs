using Discord.Commands;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YoutubeSubBot.DTO;

namespace TemplateBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("alert")]
        public async Task alert()
        {
            if (Timer.DontAlertUsers.Exists(x => x == Context.User.Id))
            {
                Timer.DontAlertUsers.Remove(Context.User.Id);
            }
            else
            {
                Timer.DontAlertUsers.Add(Context.User.Id);
            }
        }
        [Command("subscribe")]
        public async Task sub( string channel)
        {
            YoutuberDTO item = new YoutuberDTO();

            if (Timer.Youtubers.Exists(x => x.YoutuberID == channel))
            {
                var youtuber = Timer.Youtubers.Find(x => x.YoutuberID == channel);
                youtuber.Subscribers.Add(Context.User.Id);
                await Context.Channel.SendMessageAsync("User added to alerts for "+ channel);
            }
            else
            {
                YoutuberDTO Youtuber = new YoutuberDTO();
                Youtuber.YoutuberID = channel;
                List<ulong> scribers = new List<ulong>();
                scribers.Add(Context.User.Id);
                Youtuber.Subscribers = scribers;
                Timer.Youtubers.Add(Youtuber);
                await Context.Channel.SendMessageAsync( channel+ " added monitor and user is added to alerts for the channel");
            }
        }
        [Command("unsubscribe")]
        public async Task unscribe(string channel)
        {
            if (Timer.Youtubers.Exists(x=> x.YoutuberID == channel))
            {
                var youtuber = Timer.Youtubers.Find(x => x.YoutuberID == channel);
                if (youtuber.Subscribers.Exists(z=> z == Context.User.Id))
                {
                    youtuber.Subscribers.Remove(Context.User.Id);
                    await Context.Channel.SendMessageAsync("You dont are no longer subscribed to that channel!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("You dont are'nt subscribed to that channel!");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("There are no alerts setup for that channel");
            }
        }
        [Command("getsubscribers")]
        public async Task Getsubscribers(string user)
        {
            FindUsers();
        }
        [Command("showsubscribers")]
        public async Task showscribers()
        {
            string msg = "";
            foreach (var item in Timer.Youtubers.FindAll(x=>x.Subscribers.Exists(z=> z ==Context.User.Id)))
            {
                if (msg.Length > 1700)
                {
                    await Context.Channel.SendMessageAsync(msg = msg + item.YoutuberID);
                    msg = "";
                }
                else
                {
                    msg = msg + item.YoutuberID + Environment.NewLine;
                }
            }
            await Context.Channel.SendMessageAsync(msg);
        }
        [Command("Ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }
        private async void FindUsers()
        {
            string URL = "https://www.youtube.com/user/Sindre2244/channels";
            //Find all videos that have been posted within 1 hour from channel

            int i = 1;
            HtmlDocument doc = new HtmlDocument();
            HtmlWeb hw = new HtmlWeb();
            doc = hw.Load(URL);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//*[@id=""channels-browse-content-grid""]");
            foreach (var item2 in nodes)
            {
                foreach (var channel in item2.ChildNodes)
                {
                    if (channel.Name == "li")
                    {

                        foreach (var channelitem in channel.ChildNodes)
                        {

                            if (channelitem.Name == "div")
                            {
                                string output = channelitem.InnerHtml;
                                string item = Timer.StringFinder(output, "<a href=\"", "\" class=\"ux-thumb-wrap yt-uix-sessionlink");
                                await Program.Log(item + "  -  " + i, ConsoleColor.White);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}
