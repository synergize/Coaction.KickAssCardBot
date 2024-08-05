using Coaction.KickAssCardBot.Embed_Output;
using Coaction.KickAssCardBot.Enums;
using Coaction.KickAssCardBot.Models.Scryfall;
using Discord;

namespace Coaction.KickAssCardBot.Extensions
{
    public static class CardDataExtensions
    {
        public static async Task<EmbedBuilder> GetCardDataAsEmbed(this ScryfallDataModel.CardData cardInfo)
        {
            return await MtgCardOutputManager.CardOutput(cardInfo);
        }

        /// <summary>
        /// Builds the select menu that's displayed under a card to provide different printings. Only takes the first 25 due to discord embed limitations.
        /// </summary>
        /// <param name="prints"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static List<SelectMenuOptionBuilder> BuildPrintingSelectMenu(this ScryfallPrints prints, GameType gameType = GameType.Paper)
        {
            var selectOptions = prints
                .FilterOnlyPrintsWithPricingData()
                .FilterByGameType(gameType)
                .RemoveDuplicates()
                .CardData;

            return selectOptions.Take(25).Select(print => new SelectMenuOptionBuilder { Label = print.SetName, Value = print.Set }).ToList();
        }
    }
}
