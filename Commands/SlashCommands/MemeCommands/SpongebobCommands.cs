using Discord.Interactions;

namespace Coaction.KickAssCardBot.Commands.SlashCommands.MemeCommands
{
    public class SpongebobCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("meme-sponge", "We're mememing out here.")]
        public async Task CapitalizeEveryOtherOne(string message)
        {
            var newString = string.Concat(message.ToLower().AsEnumerable().Select((c, i) => i % 2 == 0 ? c : char.ToUpper(c)));
            await Context.Interaction.RespondAsync(newString);
        }
    }
}
