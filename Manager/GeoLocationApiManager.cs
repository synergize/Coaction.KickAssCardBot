using Coaction.KickAssCardBot.Models.GeoLocation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Coaction.KickAssCardBot.Manager
{
    public class GeoLocationApiManager
    {
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/";
        private readonly ILogger<GeoLocationApiManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public GeoLocationApiManager(ILogger<GeoLocationApiManager> logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<GeoLocationResults> GetLocationByZipCode(string zipCode)
        {
            try
            {
                var cacheKey = $"GeoLocation-{zipCode}";
                if (_memoryCache.TryGetValue(cacheKey, out var geoLocation))
                {
                    if (geoLocation is GeoLocationResults castedGeoLocation)
                    {
                        return castedGeoLocation;
                    }
                }

                using var client = new HttpClient()
                {
                    BaseAddress = new Uri(BaseUrl)
                };
                var response = await client.GetAsync($"json?address={zipCode}&key={_configuration["GOOGLE_MAPS_API_MTG"]}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GeoLocationResults>(content) ?? throw new Exception($"Failed To deserialize geo location results for {zipCode} from {client.BaseAddress.AbsolutePath}.");
                _memoryCache.Set(cacheKey, result);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to get geo location results for {zipCode} due to {e.Message}");
                throw;
            }
        }
    }
}
