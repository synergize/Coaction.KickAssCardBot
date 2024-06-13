using System.Runtime.Caching;
using Coaction.KickAssCardBot.Models.Scryfall;
using Newtonsoft.Json;
using static Coaction.KickAssCardBot.Models.Scryfall.ScryfallDataModel;

namespace Coaction.KickAssCardBot.Helpers.Scryfall
{
    public class GetScryFallData
    {
        private static readonly JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate
        };

        public static async Task<CardData> PullScryfallData(string cardName, string setName = "")
        {
            cardName = FormatUserInput.FormatEntry(cardName);
            var cache = MemoryCache.Default;
            Logger.Log.Info($"Pulling Scryfall Data For {cardName} via set {setName}");
            var cacheKey = $"cardData-{cardName}-{setName}";

            if (cache.Contains(cacheKey))
            {
                return cache.Get(cacheKey) as CardData;
            }

            var callApi = await ExactScryFall(cardName, setName);
            if (callApi != null)
            {
                var pullCard = JsonConvert.DeserializeObject<ScryfallDataModel.CardData>(callApi, SerializationSettings);
                if (pullCard != null)
                {
                    cache.Add(cacheKey, pullCard, DateTimeOffset.UtcNow.AddDays(1));
                    return pullCard;
                }
            }
            return null;
        }

        public static async Task<ScryFallCardRulingsModel> PullScryFallRuleData(Guid scryfallId)
        {
            var cache = MemoryCache.Default;
            var cacheKey = $"cardrules-{scryfallId}";
            if (cache.Contains(cacheKey))
            {
                return cache.Get(cacheKey) as ScryFallCardRulingsModel;
            }

            using var web = new HttpClient();
            try
            {
                var url = string.Format($"https://api.scryfall.com/cards/{scryfallId}/rulings");
                var downloadRules = await web.GetStringAsync(url);
                var rules = JsonConvert.DeserializeObject<ScryFallCardRulingsModel>(downloadRules, SerializationSettings);
                if (rules != null)
                {
                    cache.Add(cacheKey, rules, DateTimeOffset.UtcNow.AddDays(1));
                    return rules;
                }
            }
            catch (Exception msg)
            {
                Logger.Log.Error($"Failed to find ruling data. Reason: {msg.Message}");
                return null;
            }

            return null;
        }

        public static async Task<CardData> PullScryfallData(Guid scryfallId)
        {
            var cache = MemoryCache.Default;
            var cacheKey = $"carddata-{scryfallId}";
            if (cache.Contains(scryfallId.ToString()))
            {
                return cache.Get(cacheKey) as CardData;
            }

            using var web = new HttpClient();
            try
            {
                var url = new Uri(string.Format($"https://api.scryfall.com/cards/{scryfallId}"));
                var downloadRules = await web.GetStringAsync(url);
                var cardData = JsonConvert.DeserializeObject<CardData>(downloadRules, SerializationSettings);
                if (cardData != null)
                {
                    cache.Add(cacheKey, cardData, DateTimeOffset.UtcNow.AddDays(1));
                    return cardData;
                }
            }
            catch (Exception msg)
            {
                Logger.Log.Error($"Failed to find card data using Scryfall ID: {scryfallId}. Reason: {msg.Message}");
                return null;
            }

            return null;
        }

        public static async Task<ScryfallDataModel.CardData> PullScryFallRandomCard(bool isCommander)
        {
            using var web = new HttpClient();
            try
            {
                var url = string.Format($"https://api.scryfall.com/cards/random{(isCommander ? "?q=is%3Acommander" : "")}");
                var downloadRules = await web.GetStringAsync(url);
                {
                    var pulledCard = JsonConvert.DeserializeObject<ScryfallDataModel.CardData>(downloadRules, SerializationSettings);
                    if (pulledCard != null)
                    {
                        return pulledCard;
                    }
                }
                return null;
            }
            catch (Exception msg)
            {
                Logger.Log.Error(msg, $"Failed to get random scryfall card due to {msg.Message}");
                return null;
            }
        }

        public static async Task<ScryFallAutoCompleteModel> PullScryFallAutoComplete(string entry)
        {
            using var web = new HttpClient();
            try
            {
                var url = string.Format($"https://api.scryfall.com/cards/autocomplete?q={entry}");
                var downloadRules = await web.GetStringAsync(url);
                return JsonConvert.DeserializeObject<ScryFallAutoCompleteModel>(downloadRules, SerializationSettings);
            }
            catch (Exception msg)
            {
                Logger.Log.Error(msg, $"Failed to get scryfall auto complete due to {msg.Message}");
                return null;
            }
        }

        public static async Task<Symbology> PullScryfallSymbology()
        {
            string downloadNews = null;
            using (var web = new HttpClient())
            {
                try
                {
                    var url = string.Format($"https://api.scryfall.com/symbology");
                    downloadNews = await web.GetStringAsync(url);
                }
                catch (Exception msg)
                {
                    Logger.Log.Error($"Unable to access symbology API Call \n {msg}");
                    downloadNews = null;
                }
            }

            return downloadNews == null ? null : JsonConvert.DeserializeObject<Symbology>(downloadNews);
        }

        public static async Task<ScryfallPrints> PullScryfallSetData(string urlFromInitial)
        {
            using var web = new HttpClient();
            try
            {
                var url = string.Format(urlFromInitial);
                var cache = MemoryCache.Default;
                var cacheKey = $"setdata-{url}";
                if (cache.Contains(cacheKey))
                {
                    return cache.Get(cacheKey) as ScryfallPrints;
                }
                Logger.Log.Info($"Getting set data for {url}");
                var downloadRules = await web.GetStringAsync(url);
                {
                    var pulledCard = JsonConvert.DeserializeObject<ScryfallPrints>(downloadRules, SerializationSettings);
                    if (pulledCard != null)
                    {
                        cache.Add(cacheKey, pulledCard, DateTimeOffset.UtcNow.AddDays(1));
                        return pulledCard;
                    }
                }
                return null;
            }
            catch (Exception msg)
            {
                Logger.Log.Error(msg, $"Failed to get scryfall set data due to {msg.Message}");
                return null;
            }
        }

        private static async Task<string> FuzzyScryFall(string cardName, string setName = "")
        {
            string downloadNews;
            using var web = new HttpClient();
            try
            {
                var url = string.Format(string.IsNullOrEmpty(setName) ? $"https://api.scryfall.com/cards/named?fuzzy={cardName}&format=json" : $"https://api.scryfall.com/cards/named?fuzzy={cardName}&set={setName}&format=json");
                Logger.Log.Info($"Getting {url}");
                downloadNews = await web.GetStringAsync(url);
            }
            catch (Exception msg)
            {
                Logger.Log.Error(msg, $"Fuzzy failed due to {msg.Message}");
                return null;
            }

            return downloadNews;
        }

        private static async Task<string> ExactScryFall(string cardName, string setName = "")
        {
            string downloadNews = null;
            using var web = new HttpClient();
            try
            {
                var url = string.Format(string.IsNullOrEmpty(setName)
                    ? $"https://api.scryfall.com/cards/named?exact={cardName}&format=json"
                    : $"https://api.scryfall.com/cards/named?exact={cardName}&set={setName}&format=json");
                Logger.Log.Info($"Trying to get exact scryfall data for {cardName} via set {setName}. URL: {url}");
                downloadNews = await web.GetStringAsync(url);
            }
            catch (HttpRequestException exception)
            {
                Logger.Log.Error("Failed to acquire card information via exact link. Attempting Fuzzy..", exception);
                return await FuzzyScryFall(cardName, setName);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e, $"Uncaught exception due to {e.Message}");
            }

            return downloadNews;
        }
        private static Dictionary<string, string> SetLegalList(ScryfallDataModel.Legalities legalities)
        {
            var legalitiesDictionary = new Dictionary<string, string>
            {
                {"Standard", LegalityDictionary.Legality[legalities.Standard]},
                {"Modern", LegalityDictionary.Legality[legalities.Modern]},
                {"Legacy", LegalityDictionary.Legality[legalities.Legacy]},
                {"Vintage", LegalityDictionary.Legality[legalities.Vintage]},
                {"Commander", LegalityDictionary.Legality[legalities.Commander]},
                {"Pauper", LegalityDictionary.Legality[legalities.Pauper]},
                {"Pioneer", LegalityDictionary.Legality[legalities.Pioneer]},
                {"Historic", LegalityDictionary.Legality[legalities.Historic]}
            };

            return legalitiesDictionary;
        }
    }
}
