using Coaction.KickAssCardBot.Embed_Output;
using Discord.Commands;
using GetScryFallData = Coaction.KickAssCardBot.Helpers.Scryfall.GetScryFallData;

namespace Coaction.KickAssCardBot.Commands
{
    public class MTGRandomCard : ModuleBase<SocketCommandContext>
    {
        [Command("random")]
        public async Task MTGRandomCards(bool isCommander = true)
        {
            var embed = await MtgCardOutputManager.CardOutput(await GetScryFallData.PullScryFallRandomCard(isCommander));    
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
