using Coaction.KickAssCardBot.Embed_Output;
using Coaction.KickAssCardBot.Extensions;
using Coaction.KickAssCardBot.Helpers;
using Coaction.KickAssCardBot.Manager;
using Discord.Commands;
using Discord.Interactions;

namespace Coaction.KickAssCardBot.Commands.SlashCommands
{
    public class CardLookupSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ScryfallManagerService _scryfallManager;

        public CardLookupSlashCommands(ScryfallManagerService scryfallManager)
        {
            _scryfallManager = scryfallManager;
        }

        [SlashCommand("card-lookup", "Uses Scryfall to lookup information about provided card name")]
        public async Task CardLookup(string cardName)
        {
            var cardData = await _scryfallManager.PullScryfallData(cardName);
            var embedBuilder = await cardData?.GetCardDataAsEmbed();
            var scryFallSetData = await _scryfallManager.PullScryfallSetData(cardData?.PrintsSearchUri);
            var purchaseCardButtons = ButtonComponentHelper.BuildBuyButtons(cardData, selectItems: scryFallSetData?.BuildPrintingSelectMenu());

            if (embedBuilder.Length > 0)
            {
                await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), components: purchaseCardButtons.Build());
            }
        }

        [SlashCommand("card-rules", "Uses Scryfall to lookup rules about provided card name")]
        public async Task RulesLookup(string cardName)
        {
            var cardData = await _scryfallManager.PullScryfallData(cardName);
            var rulingData = await _scryfallManager.PullScryFallRuleData(cardData.Id);
            var firstRule = rulingData.Rules.FirstOrDefault();
            var embedBuilder = MtgCardOutputManager.RulingOutput(rulingData, cardData, firstRule);
            var purchaseCardButtons = ButtonComponentHelper.BuildRuleButtons(rulingData, firstRule, cardData.Id);

            if (embedBuilder.Length > 0)
            {
                await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), components: purchaseCardButtons.Build());
            }
        }

        [Command("random")]
        public async Task MTGRandomCards(bool isCommander = true)
        {
            var embed = await MtgCardOutputManager.CardOutput(await _scryfallManager.PullScryFallRandomCard(isCommander));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
