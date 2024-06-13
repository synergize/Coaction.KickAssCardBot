using Coaction.KickAssCardBot.Embed_Output;
using Discord;
using Discord.Interactions;

namespace Coaction.KickAssCardBot.Commands.SlashCommands.Help
{
    public class MtgHelpSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [UserCommand("help")]
        public async Task MtgHelpSlashCommand(IUser user)
        {
            var help = new MtgCardOutputManager();
            await Context.Interaction.RespondAsync("", embed: help.GettingHelp().Build(), ephemeral: true);
        }

        [SlashCommand("help", "Details about what this bot can do!")]
        public async Task MtgHelpSlashCommand()
        {
            var help = new MtgCardOutputManager();
            await Context.Interaction.RespondAsync("", embed: help.GettingHelp().Build(), ephemeral: true);
        }
    }
}
