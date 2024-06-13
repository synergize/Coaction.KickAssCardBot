//using Coaction.KickAssCardBot.Embed_Output;
//using Coaction.KickAssCardBot.Enums;
//using Coaction.KickAssCardBot.Factories;
//using Discord;
//using Discord.Interactions;
//using Discord.WebSocket;

//namespace Coaction.KickAssCardBot.Commands.SlashCommands.MoversShakersCommands
//{
//    public class MtgMoversShakersCommand : InteractionModuleBase<SocketInteractionContext>
//    {
//        [SlashCommand("setchannel", "Set a channel to display movers and shakers data in.")]
//        [RequireUserPermission(GuildPermission.ManageChannels)]
//        public async Task SetChannelConfiguration(ISocketMessageChannel channel)
//        {
//            var discordServerId = Context.Guild.Id;
//            var discordServerInformation = await MoversShakersDbFactory.MoversShakersConnection.ReadMoversShakersConfig(discordServerId);

//            if (discordServerInformation == null)
//            {
//                await MoversShakersDbFactory.MoversShakersConnection.UpdateServerInfo(new Models.DiscordServerChannelModel
//                {
//                    ServerId = discordServerId,
//                    ChannelId = channel.Id,
//                });
//                // Send message - New document created. 
//                await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().SetChannelSuccess(channel).Build());
//            }
//            else if (!discordServerInformation.ChannelId.Equals(channel.Id))
//            {
//                // Channel wasn't set.
//                discordServerInformation.ChannelId = channel.Id;
//                await MoversShakersDbFactory.MoversShakersConnection.UpdateServerInfo(discordServerInformation);
//                await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().SetChannelSuccess(channel).Build());
//            }
//            else
//            {
//                // Channel was already set.
//                await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().ChannelAlreadyConfiguredErrorMessage(channel.Name).Build());
//            }
//        }

//        [SlashCommand("addformatchannel", "Add a format for movers and shakers prices.")]
//        [RequireUserPermission(GuildPermission.ManageChannels)]
//        public async Task AddFormatForScrapeForChannel(string formatName)
//        {
//            await AddFormatForScrape(formatName);
//        }

//        [SlashCommand("addformatuser", "Add a format for movers and shakers prices that's delivered directly to you.")]
//        public async Task AddFormatForScrapeForUser(string formatName)
//        {
//            await AddFormatForScrape(formatName, Context.User);
//        }

//        private async Task AddFormatForScrape(string formatName, IUser user = null)
//        {
//            var userInput = formatName.ToLower();
//            var discordServerId = user?.Id ?? Context.Guild.Id;

//            if (Enum.TryParse(userInput, out MtgFormatsEnum format))
//            {
//                var serverInformation = await MoversShakersDbFactory.MoversShakersConnection.ReadMoversShakersConfig(discordServerId);

//                if (serverInformation == null)
//                {
//                    // Send message to user to have them set a channel.
//                    await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().NoConfiguredServerErrorOutput().Build());
//                }
//                else if (!serverInformation.ListOfFormats.Contains(userInput))
//                {
//                    // User didn't already have the format set.
//                    await MoversShakersDbFactory.MoversShakersConnection.AddChannelFormat(serverInformation, format);
//                    await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().AddFormatSuccess(format).Build());
//                }
//                else
//                {
//                    // User already added format.
//                    await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().FormatAlreadyExistsErrorMessage(format).Build());
//                }
//            }
//            else
//            {
//                // Let user know to check their input, not a valid format. 
//                await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().IncorrectOrUnspportedFormatError(formatName).Build());
//            }
//        }

//        [SlashCommand("removeformatchannel", "Remove a format from the movers and shakers price data.")]
//        [RequireUserPermission(GuildPermission.ManageChannels)]
//        public async Task RemoveFormatForScrapeChannel(string formatName)
//        {
//            await RemoveFormatForScrape(formatName);
//        }

//        [SlashCommand("removeformatuser", "Remove a format from the movers and shakers price data that's delivered directly to you.")]
//        [RequireUserPermission(GuildPermission.ManageChannels)]
//        public async Task RemoveFormatForScrapeUser(string formatName)
//        {
//            await RemoveFormatForScrape(formatName, Context.User);
//        }

//        private async Task RemoveFormatForScrape(string formatName, IUser user = null)
//        {
//            var userInput = formatName.ToLower();
//            var discordServerId = user?.Id ?? Context.Guild.Id;

//            if (Enum.TryParse(userInput, out MtgFormatsEnum format))
//            {
//                var serverInformation = await MoversShakersDbFactory.MoversShakersConnection.ReadMoversShakersConfig(discordServerId);

//                if (serverInformation == null)
//                {
//                    // Send message to user to have them set a channel.
//                    await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().NoConfiguredServerErrorOutput().Build());
//                    return;
//                }
//                else if (serverInformation.ListOfFormats.Contains(userInput))
//                {
//                    // User did have format in the list so we're removing.
//                    await MoversShakersDbFactory.MoversShakersConnection.RemoveChannelFormat(serverInformation, format);
//                    await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().RemoveFormatSuccess(format).Build());
//                    return;
//                }
//                else
//                {
//                    // User already removed channel.
//                    await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().FormatDoesntExistErrorMessage(format).Build());
//                }
//            }
//            else
//            {
//                // Let user know to check their input, not a valid format. 
//                await Context.Interaction.RespondAsync("", embed: new MtgMoversShakersOutput().IncorrectOrUnspportedFormatError(formatName).Build());
//            }
//        }
//    }
//}
