using Coaction.KickAssCardBot.Factories;
using Coaction.KickAssCardBot.Helpers.Scryfall;
using Coaction.KickAssCardBot.Models.Scryfall;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Coaction.KickAssCardBot.Models.Scryfall.ScryfallDataModel;

namespace Coaction.KickAssCardBot.Manager
{
    public class ScryfallManagerService
    {
        private readonly ILogger<ScryfallManagerService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings? _serializerSettings;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public ScryfallManagerService(ILogger<ScryfallManagerService> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Populate
            };

            _httpClient = ScryfallFactory.GetSryfallHttpClient();
            _cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromDays(7)
            };
        }

        public async Task<CardData> PullScryfallData(string cardName, string setName = "")
        {
            cardName = FormatUserInput.FormatEntry(cardName);
            _logger.LogInformation($"Pulling Scryfall Data For {cardName} via set {setName}");
            var cacheKey = $"cardData-{cardName}-{setName}";
            var cacheValue = _memoryCache.Get(cacheKey);
            if (cacheValue is CardData data)
            {
                return data;
            }

            var callApi = await ExactScryFall(cardName, setName);
            var result = JsonConvert.DeserializeObject<CardData>(callApi, _serializerSettings) ?? throw new Exception($"Failed to deserialize scryfall card data for {cardName} in set {setName}.");
            _memoryCache.Set(cacheKey, result, _cacheEntryOptions);
            return result;
        }

        public async Task<ScryFallCardRulingsModel> PullScryFallRuleData(Guid scryfallId)
        {
            var cacheKey = $"cardrules-{scryfallId}";
            var cacheValue = _memoryCache.Get(cacheKey);
            if (cacheValue is ScryFallCardRulingsModel data)
            {
                return data;
            }

            try
            {
                var url = string.Format($"https://api.scryfall.com/cards/{scryfallId}/rulings");
                var downloadRules = await _httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<ScryFallCardRulingsModel>(downloadRules, _serializerSettings) ?? throw new Exception($"Failed to deserialize scryfall rules data for {scryfallId}.");
                _memoryCache.Set(cacheKey, result, _cacheEntryOptions);
                return result;
            }
            catch (Exception msg)
            {
                _logger.LogError($"Failed to find ruling data. Reason: {msg.Message}");
                throw new Exception($"Failed to obtain scryfall rule data data for {scryfallId} due to {msg.Message}.", msg);
            }
        }

        public async Task<CardData> PullScryfallData(Guid scryfallId)
        {
            var cacheKey = $"carddata-{scryfallId}";
            var cacheValue = _memoryCache.Get(cacheKey);
            if (cacheValue is CardData data)
            {
                return data;
            }

            try
            {
                var url = new Uri(string.Format($"https://api.scryfall.com/cards/{scryfallId}"));
                var downloadRules = await _httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<CardData>(downloadRules, _serializerSettings) ?? throw new Exception($"Failed to deserialize scryfall card data.");
                _memoryCache.Set(cacheKey, result, _cacheEntryOptions);
                return result;
            }
            catch (Exception msg)
            {
                _logger.LogError($"Failed to find card data using Scryfall ID: {scryfallId}. Reason: {msg.Message}");
                throw new Exception($"Failed to obtain scryfall card data for {scryfallId} due to {msg.Message}.", msg);
            }
        }

        public async Task<CardData> PullScryFallRandomCard(bool isCommander)
        {
            try
            {
                var url = string.Format($"https://api.scryfall.com/cards/random{(isCommander ? "?q=is%3Acommander" : "")}");
                var downloadRules = await _httpClient.GetStringAsync(url);
                {
                    return JsonConvert.DeserializeObject<ScryfallDataModel.CardData>(downloadRules, _serializerSettings) ?? throw new Exception($"Failed to deserialize scryfall random card data."); ;
                }
            }
            catch (Exception msg)
            {
                _logger.LogError(msg, $"Failed to get random scryfall card due to {msg.Message}");
                throw new Exception($"Failed to obtain scryfall random card data due to {msg.Message}.", msg);
            }
        }

        public async Task<ScryFallAutoCompleteModel> PullScryFallAutoComplete(string entry)
        {
            try
            {
                var url = string.Format($"https://api.scryfall.com/cards/autocomplete?q={entry}");
                var downloadRules = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<ScryFallAutoCompleteModel>(downloadRules, _serializerSettings) ?? throw new Exception($"Failed to deserialize scryfall auto complete data."); ;
            }
            catch (Exception msg)
            {
                _logger.LogError(msg, $"Failed to get scryfall auto complete due to {msg.Message}");
                throw new Exception($"Failed to obtain scryfall auto complete data for {entry} due to {msg.Message}.", msg);
            }
        }

        public async Task<Symbology> PullScryfallSymbology()
        {
            string? downloadNews;
            try
            {
                var url = string.Format($"https://api.scryfall.com/symbology");
                downloadNews = await _httpClient.GetStringAsync(url);
            }
            catch (Exception msg)
            {
                _logger.LogError($"Unable to access symbology API Call \n {msg}");
                throw new Exception($"Failed to obtain scryfall symbology data.");
            }

            return JsonConvert.DeserializeObject<Symbology>(downloadNews) ??
                   throw new Exception($"Failed to deserialize scryfall symbology data.");
        }

        public async Task<ScryfallPrints> PullScryfallSetData(string urlFromInitial)
        {
            try
            {
                var url = string.Format(urlFromInitial);
                var cacheKey = $"setdata-{url}";
                var cacheValue = _memoryCache.Get(cacheKey);
                if (cacheValue is ScryfallPrints data)
                {
                    return data;
                }
                _logger.LogInformation($"Getting set data for {url}");
                var downloadRules = await _httpClient.GetStringAsync(url);
                {
                    var pulledCard = JsonConvert.DeserializeObject<ScryfallPrints>(downloadRules, _serializerSettings);
                    if (pulledCard != null)
                    {
                        _memoryCache.Set(cacheKey, pulledCard, _cacheEntryOptions);
                        return pulledCard;
                    }
                }
            }
            catch (Exception msg)
            {
                _logger.LogError(msg, $"Failed to get scryfall set data due to {msg.Message}");
                throw new Exception($"Failed to get scryfall set data data for {urlFromInitial} due to {msg.Message}", msg);
            }

            throw new Exception($"Failed to deserialize scryfall set data data for {urlFromInitial}");
        }

        private async Task<string> FuzzyScryFall(string cardName, string setName = "")
        {
            try
            {
                var url = string.Format(string.IsNullOrEmpty(setName) ? $"https://api.scryfall.com/cards/named?fuzzy={cardName}&format=json" : $"https://api.scryfall.com/cards/named?fuzzy={cardName}&set={setName}&format=json");
                _logger.LogInformation($"Getting {url}");
                return await _httpClient.GetStringAsync(url);
            }
            catch (Exception msg)
            {
                _logger.LogError(msg, $"Fuzzy failed due to {msg.Message}");
                throw new Exception($"Failed to obtain scryfall fuzzy data for {cardName} in set {setName}");
            }
        }

        private async Task<string> ExactScryFall(string cardName, string setName = "")
        {
            try
            {
                var url = string.Format(string.IsNullOrEmpty(setName)
                    ? $"https://api.scryfall.com/cards/named?exact={cardName}&format=json"
                    : $"https://api.scryfall.com/cards/named?exact={cardName}&set={setName}&format=json");
                _logger.LogInformation($"Trying to get exact scryfall data for {cardName} via set {setName}. URL: {url}");
                return await _httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError("Failed to acquire card information via exact link. Attempting Fuzzy..", exception);
                return await FuzzyScryFall(cardName, setName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Uncaught exception due to {e.Message}");
            }

            throw new Exception($"Failed to obtain scryfall data for {cardName} in set {setName}");
        }

        private Dictionary<string, string> SetLegalList(Legalities legalities)
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
