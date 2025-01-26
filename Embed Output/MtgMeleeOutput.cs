using Coaction.KickAssCardBot.Models.MtgMelee;
using Discord;

namespace Coaction.KickAssCardBot.Embed_Output
{
    public static class MtgMeleeOutput
    {
        public static EmbedBuilder OutputDecklistAggregator(Dictionary<string, TournamentResponse> tournamentResults, string roundId = "")
        {
            TournamentResponse response = null;
            string roundName = null;
            if (!string.IsNullOrEmpty(roundId))
            {
                if (long.TryParse(roundId, out var parsedRoundId))
                {
                    var specificRound = tournamentResults.FirstOrDefault(x => x.Value.Data.Any(x => x.RoundId == parsedRoundId));
                    response = specificRound.Value;
                    roundName = specificRound.Key;
                }
            }

            var targetRound = response ?? tournamentResults.FirstOrDefault().Value;
            var embedBuilder = new EmbedBuilder();
            embedBuilder.Url = $"https://melee.gg/Tournament/View/{targetRound.Data.FirstOrDefault()?.TournamentId}";
            embedBuilder.Title = $"Decklist Aggregator For Tournament {targetRound.Data.FirstOrDefault()?.TournamentId} - {roundName ?? tournamentResults.FirstOrDefault().Key}";
            var filteredByDecklist = targetRound.Data.GroupBy(x => x.Decklists.FirstOrDefault()?.DecklistName).Where(x => x.Count() > 5).OrderByDescending(x => x.Count());
            foreach (var deck in filteredByDecklist)
            {
                embedBuilder.AddField(deck.Key, deck.Count(), true);
            }

            return embedBuilder;
        }
    }
}
