using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        private readonly CommandService _service;

        public OwnerCommands(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("help")]
        [Remarks("A list of all commands")]
        public async Task HelpAsync([Remainder] string modulearg = null)
        {
            var embed = new EmbedBuilder
            {
                Color = new Color(114, 137, 218),
                Title = $"{Context.Client.CurrentUser.Username} | ToolBox Commands | Prefix: {Config.Load().Prefix}"
            };


            foreach (var module in _service.Modules)
            {
                var list = module.Commands.Select(command => $"`{Config.Load().Prefix}{command.Summary}` = {command.Remarks}").ToList();
                if (module.Commands.Count > 0)
                    embed.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = string.Join(", ", list);
                    });
            }

            await ReplyAsync("", false, embed.Build());
        }


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
