using Coaction.KickAssCardBot.Embed_Output;
using Coaction.KickAssCardBot.Helpers;
using Coaction.KickAssCardBot.Helpers.Scryfall;
using Coaction.KickAssCardBot.Models.Scryfall;
using Discord;
using Discord.Commands;

namespace Coaction.KickAssCardBot.User_Message_Handler
{
    public static class UserMessageController
    {
        /// <summary>
        /// This call needs to be refactored. We're always making an additional API when it's not necessary. Update this 
        /// </summary>
        /// <param name="matchName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<(EmbedBuilder, ComponentBuilder)> DetermineTypeOfMessage(string matchName, SocketCommandContext context)
        {
            EmbedBuilder messageOutputEmbed;
            var purchaseCardButtons = new ComponentBuilder();

            var cardData = await GetScryFallData.PullScryfallData(matchName);
            if (cardData == null)
            {
                var formattedName = FormatUserInput.FormatEntry(matchName).ToLower();

                var autoComplete = await GetScryFallData.PullScryFallAutoComplete(formattedName);

                messageOutputEmbed = autoComplete.data.Count == 0
                    ? MtgCardOutputManager.DetermineFailure(0)
                    : MtgCardOutputManager.DetermineFailure(0, autoComplete, formattedName);

                var autoCompleteResults = autoComplete.data.Select(cardName => new SelectMenuOptionBuilder {Label = cardName, Value = cardName}).ToList();
                
                if (autoCompleteResults.Count > 0)
                {
                    purchaseCardButtons.WithSelectMenu($"didyoumean-{context.Message.Author.Id}", autoCompleteResults, "Did You Mean?");
                }

                return (messageOutputEmbed, purchaseCardButtons);
            }

            if (matchName.Contains('?'))
            {
                var ruleData = await GetScryFallData.PullScryFallRuleData(cardData.Id);
                var firstRule = ruleData.Rules.FirstOrDefault();
                messageOutputEmbed = MtgCardOutputManager.RulingOutput(ruleData, cardData, firstRule);
                purchaseCardButtons = ButtonComponentHelper.BuildRuleButtons(ruleData, firstRule, cardData.Id);
            }
            else
            {
                var optionBuilder = await ButtonComponentHelper.BuildPrintingSelectMenu(cardData.PrintsSearchUri);
                purchaseCardButtons = ButtonComponentHelper.BuildBuyButtons(cardData, selectItems: optionBuilder);
                try
                {
                    messageOutputEmbed = await MtgCardOutputManager.CardOutput(cardData);
                }
                catch (Exception e)
                {
                    Logger.Log.Error(e, $"Unable to output card information via input {matchName} due to {e.Message}");
                    throw;
                }
            }

            return (messageOutputEmbed, purchaseCardButtons);
        }

        public static async Task<EmbedBuilder> GetCardDataAsEmbed(string matchName, ScryfallDataModel.CardData cardInfo = null)
        {
            var cardData = cardInfo ?? await GetScryFallData.PullScryfallData(matchName);
            
            if (cardData == null)
            {
                var formattedName = FormatUserInput.FormatEntry(matchName).ToLower();

                var autoComplete = await GetScryFallData.PullScryFallAutoComplete(formattedName);

                return autoComplete.data.Count == 0 ? MtgCardOutputManager.DetermineFailure(0) : MtgCardOutputManager.DetermineFailure(0, autoComplete, formattedName);
            }

            return await MtgCardOutputManager.CardOutput(cardData);
        }

        public static async Task<EmbedBuilder> GetCardRulingDataAsEmbed(string matchName)
        {
            var cardData = await GetScryFallData.PullScryfallData(matchName);

            if (cardData == null)
            {
                var formattedName = FormatUserInput.FormatEntry(matchName).ToLower();

                var autoComplete = await GetScryFallData.PullScryFallAutoComplete(formattedName);

                return autoComplete.data.Count == 0 ? MtgCardOutputManager.DetermineFailure(0) : MtgCardOutputManager.DetermineFailure(0, autoComplete, formattedName);
            }

            var rulingData = await GetScryFallData.PullScryFallRuleData(cardData.Id);
            var firstRule = rulingData.Rules.FirstOrDefault();
            return MtgCardOutputManager.RulingOutput(rulingData, cardData, firstRule);
        }
    }


}
