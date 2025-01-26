using Coaction.KickAssCardBot.Embed_Output;
using Coaction.KickAssCardBot.Extensions;
using Coaction.KickAssCardBot.Manager;
using Discord;
using Discord.Interactions;

namespace Coaction.KickAssCardBot.Commands.SlashCommands.TournamentCommands
{
    public class MtgMeleeCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly MtgMeleeManager _mtgMeleeManager;
        public MtgMeleeCommands(MtgMeleeManager mtgMeleeManager) {
            _mtgMeleeManager = mtgMeleeManager;
        }

        [SlashCommand("decklist-count-lookup", "Uses melee.gg to acquire data on a specific tournament")]
        public async Task TournamentAggregator(string tournamentId)
        {
            try
            {
                await Context.Interaction.RespondAsync($"Acquiring decklist data. This may take a while...");
                var results = await _mtgMeleeManager.GetTournamentResults(tournamentId);
                var componentBuilder = new ComponentBuilder();
                componentBuilder.WithSelectMenu($"mtgmelee-tournament-{tournamentId}", results.BuildRoundSelectionMenu());
                await Context.Channel.SendMessageAsync(embed: MtgMeleeOutput.OutputDecklistAggregator(results).Build(), components: componentBuilder.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [SlashCommand("top16-lookup", "Uses melee.gg to acquire the top 16 decklists from a tournmanet.")]
        public async Task Top16Results(string tournamentId)
        {
            await Context.Interaction.RespondAsync($"Acquiring decklist data. This may take a while...");
            var results = await _mtgMeleeManager.GetTournamentResults(tournamentId);
            await Context.Channel.SendMessageAsync(embed: MtgMeleeOutput.OutputTop16Decklists(results).Build());
        }
    }
}
