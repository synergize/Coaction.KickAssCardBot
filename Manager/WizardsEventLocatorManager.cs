//using Coaction.KickAssCardBot.Models.MagicEventLocator;
//using System.Runtime.Caching;
//using Newtonsoft.Json;

//namespace Coaction.KickAssCardBot.Manager
//{
//    public static class WizardsEventLocatorManager 
//    {

//        public static async Task<List<Event>> GetEventLocationByZipcode(string zipcode, int miles = 25)
//        {
//            try
//            {
//                var cache = MemoryCache.Default;
//                var filteredEvents = new List<Event>();
//                var geoLocationResults = await GeoLocationApiManager.GetLocationByZipCode(zipcode);
//                var geoLocation = geoLocationResults.Results.FirstOrDefault();
//                var meters = miles * 1609.344;
//                using var client = new HttpClient
//                {
//                    BaseAddress = new Uri("https://api.tabletop.wizards.com/event-reservations-service/events/")
//                };

//                var regionalQualifierResults = await client.GetAsync(
//                    $"search?" +
//                    $"lat={geoLocation.Geometry.Location.Latitude}&" +
//                    $"lng={geoLocation.Geometry.Location.Longitude}&" +
//                    $"isPremium=false&" +
//                    $"tag=magic:_the_gathering&" +
//                    $"maxMeters={meters}&" +
//                    $"pageSize=1000&" +
//                    $"page=0&" +
//                    $"sort=date&sortDirection=asc");

//                regionalQualifierResults.EnsureSuccessStatusCode();
//                var stringRegionalQualifierResults = await regionalQualifierResults.Content.ReadAsStringAsync();
//                var regionalQualifierEvents =
//                    JsonConvert.DeserializeObject<EventResults>(stringRegionalQualifierResults)?.Results;
//                if (regionalQualifierEvents is {Count: > 0})
//                {
//                    filteredEvents.AddRange(regionalQualifierEvents.Where(x =>
//                        x.Tags.Contains("regional_championship") ||
//                        x.Tags.Contains("regional_championship_qualifier") || x.Tags.Contains("store_championship")));

//                    foreach (var magicEvent in filteredEvents)
//                    {
//                        cache.AddOrGetExisting($"magicEvent-{magicEvent.EventId}", magicEvent, new CacheItemPolicy
//                        {
//                            AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddDays(1))
//                        });
//                    }
//                }

//                return filteredEvents;

//            }
//            catch (Exception e)
//            {
//                Logger.Log.Error(e, $"Failed to get Magic Events from zip code {zipcode} and mile range {miles} due to {e.Message}");
//                return null;
//            }
//        }

//        public static async Task<Store> GetStoreDetails(uint storeId)
//        {
//            try
//            {
//                using var client = new HttpClient
//                {
//                    BaseAddress = new Uri("https://api.tabletop.wizards.com/event-reservations-service/Organizations/")
//                };

//                var storeDetails = await client.GetAsync(
//                    $"by-ids?ids={storeId}");

//                storeDetails.EnsureSuccessStatusCode();
//                var storeDetailsResultString = await storeDetails.Content.ReadAsStringAsync();

//                return JsonConvert.DeserializeObject<List<Store>>(storeDetailsResultString)?.FirstOrDefault();
//            }
//            catch (Exception e)
//            {
//                Logger.Log.Error(e, $"Failed to get store details for {storeId} due to {e.Message}");
//                throw;
//            }
//        }

//        public static async Task<Event> GetMagicEvent(string eventId)
//        {
//            try
//            {
//                uint.TryParse(eventId, out var parsedEventId);
//                var cache = MemoryCache.Default;
//                var cacheKey = $"magicEvent-{parsedEventId}";

//                if (cache.Contains(cacheKey))
//                {
//                    if (cache.Get(cacheKey) is Event cachedEvent)
//                    {
//                        cachedEvent.Store ??= await GetStoreDetails(cachedEvent.OrganizationId);

//                        return (cache[cacheKey] = cachedEvent) as Event;
//                    }
//                }

//                using var client = new HttpClient
//                {
//                    BaseAddress = new Uri("https://api.tabletop.wizards.com/event-reservations-service/events/")
//                };

//                var eventResponse = await client.GetAsync($"{eventId}");

//                eventResponse.EnsureSuccessStatusCode();
//                var stringEventResponseResults = await eventResponse.Content.ReadAsStringAsync();
//                var magicEvent = JsonConvert.DeserializeObject<Event>(stringEventResponseResults);

//                if (magicEvent != null)
//                {
//                    magicEvent.Store = await GetStoreDetails(magicEvent.OrganizationId);
//                    Logger.Log.Debug($"Successfully acquired magic event: {magicEvent.Name}.");
//                    return (cache[cacheKey] = magicEvent) as Event;
//                }

//                return null;
//            }
//            catch (Exception e)
//            {
//                Logger.Log.Error(e, $"Failed to get event {eventId} due to {e.Message}.");
//                throw;
//            }
//        }
//    }
//}
