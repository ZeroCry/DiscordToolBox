using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace CoreBot.Commands
{
    [RequireOwner]
    public class OwnerCommands : ModuleBase<SocketCommandContext>
    {
        [Command("SetGame")]
        [Summary("SetGame <game>")]
        [Remarks("Set the bot's Current Game.")]
        public async Task Setgame([Remainder] string game = null)
        {
            if (game == null)
            {
                await ReplyAsync("Please specify a game");
            }
            else
            {
                try
                {
                    await (Context.Client).SetGameAsync(game);
                    await ReplyAsync($"{Context.Client.CurrentUser.Username}'s game has been set to:\n" +
                                     $"{game}");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}\n" +
                                     $"Unable to set the game");
                }

            }
        }

        [Command("Rename")]
        [Summary("Rename <newname>")]
        [Remarks("Set your bots username")]
        public async Task Rename([Remainder] string newname = null)
        {
            if (newname == null)
            {
                await ReplyAsync("Please specify a name");
            }
            else
            {
                try
                {
                    await (Context.Client.CurrentUser).ModifyAsync(x =>
                    {
                        x.Username = newname;
                    });
                    await ReplyAsync($"{Context.Client.CurrentUser.Username}'s username has been set to:\n" +
                                     $"{newname}");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}\n" +
                                     $"Unable to set the name");
                }

            }
        }

        [Command("Stats")]
        [Summary("Stats")]
        [Remarks("Display Bot Statistics")]
        public async Task BotStats()
        {
            var embed = new EmbedBuilder();

            var heap = Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString(CultureInfo.InvariantCulture);
            var uptime = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");

            embed.AddField($"{Context.Client.CurrentUser.Username} Statistics", 
                $"Servers: {Context.Client.Guilds.Count}\n" +
                $"Users: {Context.Client.Guilds.Select(x => x.Users.Count).Sum()}\n" +
                $"Unique Users: {Context.Client.Guilds.SelectMany(x => x.Users.Select(y => y.Id)).Distinct().Count()}\n" +
                $"Server Channels: {Context.Client.Guilds.Select(x => x.Channels.Count).Sum()}\n" +
                $"DM Channels: {Context.Client.DMChannels.Count}\n\n" +
                $"Uptime: {uptime}\n" +
                $"Heap Size: {heap}\n" +
                $"Discord Version: {DiscordConfig.Version}");

            await ReplyAsync("", false, embed.Build());
        }
    }
}
