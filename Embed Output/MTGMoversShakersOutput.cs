//using Coaction.KickAssCardBot.Enums;
//using Coaction.KickAssCardBot.Extensions;
//using Coaction.KickAssCardBot.Factories;
//using Coaction.KickAssCardBot.Models;
//using Discord;
//using Discord.WebSocket;
//using Newtonsoft.Json;

//namespace Coaction.KickAssCardBot.Embed_Output
//{
//    public class MtgMoversShakersOutput
//    {
//        private readonly Color _successfulColor = Color.DarkGreen;
//        private const int FailedColor = 16580608;
//        private const string FooterMessage = "Contact Coaction#5994 for any bugs.";
//        private EmbedBuilder GetDailyIncreaseMoversOutput(FormatDataModel cardsList, MtgFormatsEnum format, DateTime nextScrapeTime)
//        {
//            var titleUrl = $"https://www.mtggoldfish.com/movers/paper/{format}";
//            var buildEmbed = new EmbedBuilder();
//            buildEmbed.WithUrl(titleUrl.ToLower());
//            buildEmbed.Title = $"Daily Price Winners for {format.CapitalizeFirstLetter()}!";
//            buildEmbed.Color = _successfulColor;
//            buildEmbed.WithFooter($"Next Price Delivery");
//            buildEmbed.WithTimestamp(nextScrapeTime);
//            foreach (var item in cardsList.DailyWinners.Take(10))
//            {
//                buildEmbed.AddField($"__{item.Name}__", $"Change: {item.PriceChange} \nPrice: {item.TotalPrice} \nPercentage: {item.ChangePercentage}", true);
//            }
//            return buildEmbed;
//        }

//        private EmbedBuilder GetDailyDecreaseMoversOutput(FormatDataModel cardsList, MtgFormatsEnum format, DateTime nextScrapeTime)
//        {
//            var titleUrl = $"https://www.mtggoldfish.com/movers/paper/{format}";
//            var buildEmbed = new EmbedBuilder();
//            buildEmbed.WithUrl(titleUrl.ToLower());
//            buildEmbed.Title = $"Daily Price Losers for {format.CapitalizeFirstLetter()}!";
//            buildEmbed.Color = Color.LightOrange;
//            buildEmbed.WithFooter($"Next Price Delivery");
//            buildEmbed.WithTimestamp(nextScrapeTime);
//            foreach (var item in cardsList.DailyLosers.Take(10))
//            {
//                buildEmbed.AddField($"__{item.Name}__", $"Change: {item.PriceChange} \nPrice: {item.TotalPrice} \nPercentage: {item.ChangePercentage}", true);
//            }
//            return buildEmbed;
//        }

//        //private EmbedBuilder GetWeeklyDecreaseMoversOutput(MoverCardDataModel cardsList)
//        //{
//        //    cardsList.Format = char.ToUpper(cardsList.Format[0]) + cardsList.Format.Substring(1);
//        //    EmbedBuilder BuildEmbed = new EmbedBuilder();
//        //    BuildEmbed.Title = $"Weekly Price Losers for {cardsList.Format}!";
//        //    BuildEmbed.Color = Color.LightOrange;
//        //    BuildEmbed.WithFooter(footerMessage);
//        //    BuildEmbed.WithTimestamp(cardsList.PageLastUpdated);
//        //    foreach (var item in cardsList.WeeklyDecreaseList)
//        //    {
//        //        BuildEmbed.AddField($"__{item.Name}__", $"Change: {item.PriceChange} \nPrice: ${item.TotalPrice} \nPercentage: {item.ChangePercentage}", true);
//        //    }
//        //    return BuildEmbed;
//        //}

//        //private EmbedBuilder GetWeeklyIncreaseMoversOutput(MoverCardDataModel cardsList)
//        //{
//        //    cardsList.Format = char.ToUpper(cardsList.Format[0]) + cardsList.Format.Substring(1);
//        //    EmbedBuilder BuildEmbed = new EmbedBuilder();
//        //    BuildEmbed.Title = $"Weekly Price Winners for {cardsList.Format}!";
//        //    BuildEmbed.Color = successfulColor;
//        //    BuildEmbed.WithFooter(footerMessage);
//        //    foreach (var item in cardsList.WeeklyIncreaseList)
//        //    {
//        //        BuildEmbed.AddField($"__{item.Name}__", $"Change: {item.PriceChange} \nPrice: ${item.TotalPrice} \nPercentage: {item.ChangePercentage}", true);
//        //    }
//        //    return BuildEmbed;
//        //}

//        public EmbedBuilder NoConfiguredServerErrorOutput()
//        {
//            return new EmbedBuilder
//            {
//                Title = "Configuration Error",
//                Description = "Format modification failed due to lack of configuration. Please type !mtgsetchannel #<channel name> then try again.",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = new Color(FailedColor)
//            };
//        }

//        public EmbedBuilder NoConfiguredServerErrorOutputDeliverCards()
//        {
//            return new EmbedBuilder
//            {
//                Title = "Configuration Error",
//                Description = "There was a problem finding your configured channel. I was able to deliver the prices to your default channel but please type !mtgsetchannel #<channel name> for this message to disappear or contact my developer.",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = new Color(FailedColor)
//            };
//        }

//        public EmbedBuilder IncorrectOrUnspportedFormatError(string userInput)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Format Entry Error",
//                Description = $"Format \"{userInput}\" was unable to be accepted. This was due to unsupported format or a typo. Please double check your entry and try again. Supported formats are located here: [MTG Gold Fish Movers And Shakers](https://www.mtggoldfish.com/movers/standard)",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = new Color(FailedColor)
//            };
//        }

//        public EmbedBuilder SetChannelSuccess(ISocketMessageChannel channel)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Channel Successfully Updated",
//                Description = $"You've successfully set {channel.Name}! If you already have format(s) configured then the channel will update when MTGGoldFish updates. Otherwise, make sure to add a format via !mtgaddformat <formatname>.",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = _successfulColor
//            };
//        }

//        public EmbedBuilder AddFormatSuccess(MtgFormatsEnum formatName)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Formats Successfully Updated!",
//                Description = $"You've successfully added {formatName} to your list of supported formats. You'll see the format display within 30 minutes!",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = _successfulColor
//            };
//        }

//        public EmbedBuilder RemoveFormatSuccess(MtgFormatsEnum formatName)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Format Successfully Removed",
//                Description = $"You've successfully removed {formatName} from your list of supported formats. You'll no longer see it update.",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = _successfulColor
//            };
//        }

//        public EmbedBuilder ChannelAlreadyConfiguredErrorMessage(string channelName)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Channel Already Exists",
//                Description = $"{channelName.CapitalizeFirstLetter()} is already configured as your destination channel. Please choose another or let me do my thing!",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = new Color(FailedColor)
//            };
//        }

//        public EmbedBuilder FormatAlreadyExistsErrorMessage(MtgFormatsEnum formatName)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Format Already Exists",
//                Description = $"{formatName.CapitalizeFirstLetter()} is already configured. Please choose another format or skedaddle!",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = new Color(FailedColor)
//            };
//        }

//        public EmbedBuilder FormatDoesntExistErrorMessage(MtgFormatsEnum formatName)
//        {
//            return new EmbedBuilder
//            {
//                Title = "Format Doesn't Exist",
//                Description = $"{formatName.CapitalizeFirstLetter()} isn't configured for your server. Please double check your entry or scram!",
//                Timestamp = DateTime.Now,
//                Footer = new EmbedFooterBuilder() { Text = FooterMessage },
//                Color = new Color(FailedColor)
//            };
//        }

//        public async Task DeliverMoversOutputAsync(DiscordSocketClient Client, MtgFormatsEnum format, ulong guild, MoversShakersRabbitModel.Root cardData)
//        {
//            var guildConfig = await MoversShakersDbFactory.MoversShakersConnection.ReadMoversShakersConfig(guild);
//            FormatDataModel formattedData;
//            switch (format)
//            {
//                case MtgFormatsEnum.standard:
//                    formattedData = new FormatDataModel(cardData.Standard);
//                    break;
//                case MtgFormatsEnum.modern:
//                    formattedData = new FormatDataModel(cardData.Modern);
//                    break;
//                case MtgFormatsEnum.pioneer:
//                    formattedData = new FormatDataModel(cardData.Pioneer);
//                    break;
//                case MtgFormatsEnum.pauper:
//                    formattedData = new FormatDataModel(cardData.Pauper);
//                    break;
//                case MtgFormatsEnum.legacy:
//                    formattedData = new FormatDataModel(cardData.Legacy);
//                    break;
//                case MtgFormatsEnum.vintage:
//                    formattedData = new FormatDataModel(cardData.Vintage);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
//            }

//            var user = await Client.GetUserAsync(guild);
//            if (user != null)
//            {
//                try
//                {
//                    if (formattedData.DailyWinners.Count > 0)
//                    {
//                        await user.SendMessageAsync("", false, GetDailyIncreaseMoversOutput(formattedData, format, cardData.LastUpdatedAt).Build());
//                        Thread.Sleep(3000);
//                    }
//                    if (formattedData.DailyLosers.Count > 0)
//                    {
//                        await user.SendMessageAsync("", false, GetDailyDecreaseMoversOutput(formattedData, format, cardData.LastUpdatedAt).Build());
//                        Thread.Sleep(3000);
//                    }
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine($"Failed to send message to {user.Username} ({guild}) due to {e.Message}", e);
//                }
//            }
//            else
//            {
//                if (!(Client.GetGuild(guildConfig.ServerId).GetChannel(guildConfig.ChannelId) is IMessageChannel channel))
//                {
//                    var defaultChannel = Client.GetGuild(guildConfig.ServerId).DefaultChannel;
//                    channel = Client.GetGuild(guildConfig.ServerId).GetChannel(defaultChannel.Id) as IMessageChannel;
//                    await defaultChannel.SendMessageAsync("", false, NoConfiguredServerErrorOutputDeliverCards().Build());
//                }

//                if (formattedData.DailyWinners.Count > 0)
//                {
//                    await channel?.SendMessageAsync("", false, GetDailyIncreaseMoversOutput(formattedData, format, cardData.NextScrapeTime).Build());
//                    Thread.Sleep(3000);
//                }
//                if (formattedData.DailyLosers.Count > 0)
//                {
//                    await channel?.SendMessageAsync("", false, GetDailyDecreaseMoversOutput(formattedData, format, cardData.NextScrapeTime).Build());
//                    Thread.Sleep(3000);
//                }
//            }

//            //if (scrapedData.WeeklyIncreaseList.Count > 0)
//            //{
//            //    await channel.SendMessageAsync("", false, GetWeeklyIncreaseMoversOutput(scrapedData).Build());
//            //    Thread.Sleep(3000);
//            //}
//            //if (scrapedData.WeeklyDecreaseList.Count > 0)
//            //{
//            //    await channel.SendMessageAsync("", false, GetWeeklyDecreaseMoversOutput(scrapedData).Build());
//            //}

//        }

//        /// <summary>
//        /// Function that uses <paramref name="client"/> to help aid in sending messages to a channel.
//        /// We use 
//        /// </summary>
//        /// <param name="client"></param>
//        /// <param name="rabbitPayload"></param>
//        public async void DeliverMoversAndShakersToChannels(DiscordSocketClient client, string rabbitPayload)
//        {
//            var serverInformation = await MoversShakersDbFactory.MoversShakersConnection.GetRegisteredServers();

//            foreach (var guild in serverInformation.ListOfRegisteredDiscordGuilds)
//            {
//                var currentGuildInformation = await MoversShakersDbFactory.MoversShakersConnection.ReadMoversShakersConfig(guild);                   

//                foreach (var format in currentGuildInformation.ListOfFormats)
//                {
//                    var cardData = JsonConvert.DeserializeObject<MoversShakersRabbitModel.Root>(rabbitPayload);
//                    await DeliverMoversOutputAsync(client, format.ToEnum(), guild, cardData);
//                }                
//            }
//            Logger.Log.Info("### Movers And Shakers Delivery Successfully Completed. ###");
//        }
//    }
//}
