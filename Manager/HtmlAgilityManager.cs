using Coaction.KickAssCardBot.Factories;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Coaction.KickAssCardBot.Manager
{
    public class HtmlAgilityManager
    {
        private ILogger<HtmlAgilityManager> _logger;
        public HtmlAgilityManager(ILogger<HtmlAgilityManager> logger)
        {
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> GetAllRoundIds(string tournamentId)
        {
            _logger.LogInformation($"Acquiring round IDs for tournament {tournamentId}");
            var rounds = new Dictionary<string, string>();
            string url = $"https://melee.gg/Tournament/View/{tournamentId}";

            using var httpClient = HttpClientFactory.GetValidMtgMeleeClient();
            string html = await httpClient.GetStringAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var parentDiv = doc.DocumentNode.SelectSingleNode($"//div[@id='standings-round-selector-container']");

            if (parentDiv != null)
            {
                var buttons = parentDiv.SelectNodes(".//button");

                if (buttons != null)
                {
                    foreach (var button in buttons)
                    {
                        string dataId = button.GetAttributeValue("data-id", "N/A");
                        var dataName = button.GetAttributeValue("data-name", "N/A");
                        rounds[dataName] = dataId;
                    }
                }
                else
                {
                    _logger.LogInformation("No buttons found in the div.");
                }
            }
            else
            {
                _logger.LogError("Parent div not found.");
            }


            return rounds;
        }
    }
}
