using Coaction.KickAssCardBot.Embed_Output;
using Coaction.KickAssCardBot.Extensions;
using Coaction.KickAssCardBot.Helpers;
using Coaction.KickAssCardBot.Manager;
using Coaction.KickAssCardBot.Models.Scryfall;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Coaction.KickAssCardBot.Services
{
    public class InteractionHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InteractionHandlingService> _logger;
        private readonly ScryfallManagerService _scryfallManagerService;

        public InteractionHandlingService(ILogger<InteractionHandlingService> logger, DiscordSocketClient client, InteractionService interactionService,
            IServiceProvider serviceProvider, ScryfallManagerService scryfallManagerService)
        {
            _client = client;
            _interactionService = interactionService;
            _serviceProvider = serviceProvider;
            _scryfallManagerService = scryfallManagerService;
            _logger = logger;

            _client.Ready += ClientOnReadyAsync;
        }

        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        }

        public async Task ClientOnReadyAsync()
        {
            if (_client.Guilds.Count > 0)
            {
                foreach (var guild in _client.Guilds)
                {
                    await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
                }
            }
            else
            {
                await _interactionService.RegisterCommandsGloballyAsync();
            }
        }

        public async Task HandleInteractionAsync(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to handle Interaction due to: {e.Message}");
            }
        }

        public async Task Client_SelectMenuExecuted(SocketMessageComponent arg)
        {
            if (arg.Data.CustomId.Contains("cardset-"))
            {
                var cardId = arg.Data.CustomId.Replace("cardset-", "");
                var cardInfo = await _scryfallManagerService.PullScryfallData(cardId, arg.Data.Values.FirstOrDefault());
                var purchaseCardButtons = ButtonComponentHelper.BuildBuyButtons(cardInfo, false);
                await arg.RespondAsync(embed: cardInfo.GetCardDataAsEmbed().Build(), components: purchaseCardButtons.Build(), ephemeral: true);
            }

            if (arg.Data.CustomId.Contains("didyoumean-"))
            {
                var authorId = ulong.Parse(arg.Data.CustomId.Replace("didyoumean-", ""));
                if (arg.User.Id == authorId)
                {
                    var cardInfo = await _scryfallManagerService.PullScryfallData(arg.Data.Values.FirstOrDefault());
                    var scryFallSetData = await _scryfallManagerService.PullScryfallSetData(cardInfo?.PrintsSearchUri);
                    var purchaseCardButtons = ButtonComponentHelper.BuildBuyButtons(cardInfo, selectItems: scryFallSetData.BuildPrintingSelectMenu());
                    await arg.UpdateAsync(properties =>
                    {
                        properties.Components = purchaseCardButtons.Build();
                        properties.Embed = cardInfo.GetCardDataAsEmbed().Build();
                    });
                }
                else
                {
                    await arg.DeferAsync(true);
                }
            }

            //if (arg.Data.CustomId.Contains($"magicevent-"))
            //{
            //    try
            //    {
            //        var eventId = arg.Data.Values.First().Replace($"magicevent-", "");
            //        var magicEvent = await WizardsEventLocatorManager.GetMagicEvent(eventId);
            //        if (magicEvent != null)
            //        {
            //            var embed = new EmbedBuilder
            //            {
            //                Title = magicEvent.Name,
            //                Description = $"{magicEvent.Description.Replace("\n", "")} \n ```{magicEvent.Store.PostalAddress.Replace("\n", " ")}```",
            //                Url = $"https://locator.wizards.com/events/{eventId}",
            //                Color = Color.DarkGreen,
            //                Timestamp = magicEvent.StartDatetime,
            //                Footer = new EmbedFooterBuilder
            //                {
            //                    Text = "Event Date"
            //                }
            //            };

            //            var formattedFormat = magicEvent.Format.ToLower();
            //            embed.AddField($"{nameof(magicEvent.Distance)}", $"{Math.Round(magicEvent.Distance.ConvertToMiles(), 2)} Miles", true);
            //            embed.AddField($"{nameof(magicEvent.Format)}", $"{char.ToUpper(formattedFormat[0])}{formattedFormat[1..]}", true);
            //            embed.AddField($"{nameof(magicEvent.Store.Name)}", magicEvent.Store.Name, true);
            //            embed.AddField($"{nameof(magicEvent.Currency)}", $"${magicEvent.Cost * 0.01} {magicEvent.Currency}", true);
            //            embed.AddField($"{nameof(magicEvent.Store.PhoneNumber)}", magicEvent.Store.PhoneNumber, true);
            //            embed.AddField($"{nameof(magicEvent.Store.Website)}", magicEvent.Store.Website, true);
            //            await arg.RespondAsync(embed: embed.Build(), ephemeral: true);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Log.Error(e, $"Failed to find event details from selection menu due to {e.Message}");
            //        throw;
            //    }
            //}
        }

        public async Task<Task> Client_ButtonExecuted(SocketMessageComponent component)
        {
            var customId = component.Data.CustomId;
            if (customId.Contains("rules-"))
            {
                var guidFromComponent = Guid.Parse(customId.Replace("rules-", ""));
                var cardData = await _scryfallManagerService.PullScryfallData(guidFromComponent);
                var cardRules = await _scryfallManagerService.PullScryFallRuleData(guidFromComponent);
                var firstRule = cardRules.Rules.FirstOrDefault();
                await component.RespondAsync(embed: MtgCardOutputManager.RulingOutput(cardRules, cardData, firstRule).Build(), components: ButtonComponentHelper.BuildRuleButtons(cardRules, firstRule, cardData.Id).Build(), ephemeral: true);
            }

            // Logic setup to handle the pagination of Rules when "Next" or "Previous" buttons are pressed on a ruling output.
            if (customId.Contains("CardRule-"))
            {
                var indexCharacterStart = customId.IndexOf('#');
                var index = customId.Substring(indexCharacterStart + 1);
                if (int.TryParse(index, out var parsedIndex))
                {
                    var oracleId = Guid.Parse(component.Data.CustomId.Replace("CardRule-Next-", "").Replace("CardRule-Previous-", "").Replace("CardRule-Send-", "").Replace(customId.Substring(indexCharacterStart), ""));
                    var rules = await _scryfallManagerService.PullScryFallRuleData(oracleId);
                    var cardData = await _scryfallManagerService.PullScryfallData(oracleId);
                    if (customId.Contains("CardRule-Send-"))
                    {
                        await component.RespondAsync(embed: MtgCardOutputManager.RulingOutput(rules, cardData, rules.Rules[parsedIndex]).Build());
                        return Task.CompletedTask;
                    }
                    ScryFallCardRulingsModel.Rule rule = null;

                    if (customId.Contains("Next"))
                    {
                        rule = rules.Rules[parsedIndex + 1];
                    }

                    if (customId.Contains("Previous"))
                    {
                        rule = rules.Rules[parsedIndex - 1];
                    }

                    await component.RespondAsync(
                        embed: MtgCardOutputManager.RulingOutput(rules, cardData, rule).Build(),
                        components: ButtonComponentHelper.BuildRuleButtons(rules, rule, cardData.Id).Build(), ephemeral: true);

                    return Task.CompletedTask;
                }
            }

            if (customId.Contains($"legalities-"))
            {

                var guidFromComponent = Guid.Parse(customId.Replace("legalities-", ""));
                var cardData = await _scryfallManagerService.PullScryfallData(guidFromComponent);
                await component.RespondAsync(embed: MtgCardOutputManager.LegalitiesOutput(cardData).Build(), ephemeral: true);
            }

            return Task.CompletedTask;
        }
    }
}
