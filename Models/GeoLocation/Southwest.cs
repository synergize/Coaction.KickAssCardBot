using Newtonsoft.Json;

namespace Coaction.KickAssCardBot.Models.GeoLocation
{
    public class Southwest
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}
