using Coaction.KickAssCardBot.Embed_Output;
using Coaction.KickAssCardBot.Helpers;
using Coaction.KickAssCardBot.Helpers.Scryfall;
using Coaction.KickAssCardBot.User_Message_Handler;
using Discord.Interactions;

namespace Coaction.KickAssCardBot.Commands.SlashCommands
{
    public class CardLookupSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("card-lookup", "Uses Scryfall to lookup information about provided card name")]
        public async Task CardLookup(string cardName)
        {
            var cardData = await GetScryFallData.PullScryfallData(cardName);
            var embedBuilder = await UserMessageController.GetCardDataAsEmbed(cardName, cardData);
            var optionBuilder = await ButtonComponentHelper.BuildPrintingSelectMenu(cardData.PrintsSearchUri);
            var purchaseCardButtons = ButtonComponentHelper.BuildBuyButtons(cardData, selectItems: optionBuilder);

            if (embedBuilder.Length > 0)
            {
                await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), components: purchaseCardButtons.Build());
            }
        }

        [SlashCommand("card-rules", "Uses Scryfall to lookup rules about provided card name")]
        public async Task RulesLookup(string cardName)
        {
            var cardData = await GetScryFallData.PullScryfallData(cardName);
            var rulingData = await GetScryFallData.PullScryFallRuleData(cardData.Id);
            var firstRule = rulingData.Rules.FirstOrDefault();
            var embedBuilder = MtgCardOutputManager.RulingOutput(rulingData, cardData, firstRule);
            var purchaseCardButtons = ButtonComponentHelper.BuildRuleButtons(rulingData, firstRule, cardData.Id);

            if (embedBuilder.Length > 0)
            {
                await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), components: purchaseCardButtons.Build());
            }
        }
    }
}
