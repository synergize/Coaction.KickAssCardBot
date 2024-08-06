using Coaction.KickAssCardBot.Embed_Output;
using Discord;
using Discord.Interactions;

namespace Coaction.KickAssCardBot.Commands.SlashCommands.Help
{
    public class MtgHelpSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly MtgCardOutputManager _mtgCardOutputManager;

        public MtgHelpSlashCommands(MtgCardOutputManager mtgCardOutputManager)
        {
            _mtgCardOutputManager = mtgCardOutputManager;
        }

        [UserCommand("help")]
        public async Task MtgHelpSlashCommand(IUser user)
        {
            await Context.Interaction.RespondAsync("", embed: _mtgCardOutputManager.GettingHelp().Build(), ephemeral: true);
        }

        [SlashCommand("help", "Details about what this bot can do!")]
        public async Task MtgHelpSlashCommand()
        {
            await Context.Interaction.RespondAsync("", embed: _mtgCardOutputManager.GettingHelp().Build(), ephemeral: true);
        }
    }
}
